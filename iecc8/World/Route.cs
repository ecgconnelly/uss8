using Iecc8.Messages;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel.Description;
using System.Threading.Tasks;

namespace Iecc8.World {
	/// <summary>
	/// A route.
	/// </summary>
	public class Route {
		#region Common API
		/// <summary>
		/// The signal at which this route starts.
		/// </summary>
		public readonly ControlledSignal Entrance;

		/// <summary>
		/// The signal at which this route ends.
		/// </summary>
		public readonly Signal Exit;

		/// <summary>
		/// The divergence level of this route (i.e. which head the immediate aspect for the route itself is shown on).
		/// </summary>
		public readonly byte Divergence;

		/// <summary>
		/// Whether the entrance signal displays a restricting aspect when this route is set.
		/// </summary>
		public readonly bool Restricting;

		/// <summary>
		/// How this route, if diverging, calculates its aspect in terms of routes ahead.
		/// </summary>
		/// <remarks>
		/// If this field is <c>false</c> (the default), the lower aspect is calculated based on the total number of blocks ahead that are available up to the next signal at stop; for example, if the next signal shows red over yellow, then this signal will show red over flashing yellow. If this field is <c>true</c>, the lower aspect is calculated based only on the number of blocksare available up to the next signal at stop or diverging; for example, if the next signal shows red over yellow, then this signal will also show red over yellow because only one block is available until the next divergence.
		/// </remarks>
		public readonly bool DivergenceDistanceStraightOnly;

		/// <summary>
		/// The track circuits along this route.
		/// </summary>
		public readonly IReadOnlyList<RouteElement> Elements;

		/// <summary>
		/// The points along this route and what position they need to be in.
		/// </summary>
		public readonly IReadOnlyList<RoutePointPosition> PointPositions;

		/// <summary>
		/// The track circuits that need to be free in order for this route to be available, but which are not actually part of the route.
		/// </summary>
		public readonly IReadOnlyList<TrackCircuit> FreeTrackCircuits;
		#endregion

		#region Signaller API

		public bool PointsProved
		{
			get
			{
				foreach (RoutePointPosition rpp in PointPositions)
				{
					if (!rpp.Points.Proved || (rpp.Points.Reversed != rpp.Reverse)) return false;
				}
				return true;
			}
		}

		/// <summary>
		/// Whether this route is able to be called right now.
		/// </summary>
		public bool Available {
			get {
				Debug.Print("Available??");
				// If the entrance signal already has a different route, you can't set another one.
				if ((Entrance.CurrentRoute != null) && Entrance.CurrentRoute != this) {
					Debug.Print("This signal already has route: {0} --> {1}",
						Entrance.ID,
						Entrance.CurrentRoute.Exit.ID);

					return false;
				}

				// If the entrance signal is in flag-by mode, you can't set a route. You must clear the flag-by status first.
				if (Entrance.FlagBy) {
					return false;
				}

				// All occupied or locked track circuits must be locked in the same direction as this route requires it to prevent two exactly opposite routes from being called.
				foreach (RouteElement i in Elements) {
					TrackCircuit tc = i.TrackCircuit;
					if ((tc.RouteLocked || tc.Occupied) && tc.RouteLockedDirection != '\0' && tc.RouteLockedDirection != i.Direction) {
						Debug.Print("TC {0} is route locked or occupied",
							tc.ID);
						return false;
					}
				}

				// All track circuits that are required to be free must be so.
				foreach (TrackCircuit i in FreeTrackCircuits) {
					if (i.Occupied)
					{
						Debug.Print("FTC {0} is occupied", i.ID);
						return false;
					}

					if (i.RouteLocked && i.LockedRoute != this) {
                        Debug.Print("FTC {0} is route locked by a different route",
                            i.ID);
                        return false;
					}
				}

				// All points in the route must be in power-operated mode and be able to reach their required positions.
				foreach (RoutePointPosition i in PointPositions) {
					if (i.Points.HandCrankable) {
						return false;
					}
					if (!i.Points.Movable && i.Points.Reversed != i.Reverse) {
						Debug.Print("Switch {0} is not movable right now",
							i.Points.ID);
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Calls the route.
		/// </summary>
		/// <remarks>
		/// The caller must verify that the route is available first.
		/// </remarks>
		public async Task CallAsync(ESignalIndication requestedIndication = ESignalIndication.Proceed) {
			bool fleet = requestedIndication == ESignalIndication.Fleet;

            Debug.Assert(Available);

			// Lock the track circuits.
			for (int i = 0; i != Elements.Count; ++i) {
				TrackCircuit current = Elements[i].TrackCircuit;
				TrackCircuit next = i + 1 < Elements.Count ? Elements[i + 1].TrackCircuit : null;
				current.RouteLock(Elements[i].Direction, next, this);
			}

			// Set the signal's route.
			Entrance.SetCurrentRoute(this, requestedIndication);

			// Swing and lock the points.
			Task[] tasks = new Task[PointPositions.Count];
			for (int i = 0; i != PointPositions.Count; ++i) {
				tasks[i] = PointPositions[i].Points.SwingAsync(PointPositions[i].Reverse);
			}
			foreach (Task i in tasks) {
				await i;
			}
		}
		#endregion


		#region Data Initialization API
		/// <summary>
		/// Constructs a route from its spec.
		/// </summary>
		/// <param name="schema">The schema object containing data about this route.</param>
		/// <param name="region">The region the route exists in.</param>
		/// <param name="entrance">The signal at which this route begins.</param>
		public Route(Schema.Route schema, Region region, ControlledSignal entrance) {
			Entrance = entrance;
			Exit = region.GetSignal(schema.Exit, entrance.SubArea);
			Divergence = schema.Divergence;
			Restricting = schema.Restricting;
			DivergenceDistanceStraightOnly = schema.DivergenceDistanceStraightOnly;
			{
				RouteElement[] elts = new RouteElement[schema.TCs.Count];
				for (int i = 0; i != schema.TCs.Count; ++i) {
					elts[i] = new RouteElement(schema.TCs[i], region, entrance.SubArea);
				}
				Elements = Array.AsReadOnly(elts);
			}
			{
				RoutePointPosition[] elts = new RoutePointPosition[schema.Points.Count];
				for (int i = 0; i != schema.Points.Count; ++i) {
					elts[i] = new RoutePointPosition(schema.Points[i], region, entrance.SubArea);
				}
				PointPositions = Array.AsReadOnly(elts);
			}
			{
				TrackCircuit[] tcs = new TrackCircuit[schema.FreeTCs.Count];
				for (int i = 0; i != schema.FreeTCs.Count; ++i) {
					tcs[i] = region.GetTrackCircuit(schema.FreeTCs[i], entrance.SubArea);
				}
				FreeTrackCircuits = Array.AsReadOnly(tcs);
			}
		}
		#endregion
	}
}
