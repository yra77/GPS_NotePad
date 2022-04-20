
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;


namespace GPS_NotePad.Models
{
    //TransitDetails
    public class TransitDetails
    {

        [JsonProperty("arrival_stop")]
        public TransitStop ArrivalStop { get; set; }

        [JsonProperty("arrival_time")]
        public TimeZoneTextValueObject ArrivalTime { get; set; }

        [JsonProperty("departure_stop")]
        public TransitStop DepartureStop { get; set; }

        [JsonProperty("departure_time")]
        public TimeZoneTextValueObject DepartureTime { get; set; }

        [JsonProperty("headsign")]
        public string HeadSign { get; set; }

        [JsonProperty("headway")]
        public long HeadWay { get; set; }

        [JsonProperty("line")]
        public TransitLine Line { get; set; }

        [JsonProperty("num_stops")]
        public long NumStops { get; set; }

        [JsonProperty("trip_short_name")]
        public string TripShortName { get; set; }
    }

    public class TransitStop
    {
        [JsonProperty("location")]
        public LatLngLiteral Location { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class TimeZoneTextValueObject
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }
    }

    public class TransitLine
    {
        [JsonProperty("agencies")]
        public IList<TransitAgency> Agencies { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("short_name")]
        public string Short_Name { get; set; }

        [JsonProperty("text_color")]
        public string TextColor { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("vehicle")]
        public TransitVehicle Vehicle { get; set; }
    }

    public class TransitAgency
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class TransitVehicle
    {
        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public class LatLngLiteral
    {
        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }
    //End TransitDetails

    public class GeocodedWaypoint
    {
        [JsonProperty("geocoder_status")]
        public string GeocoderStatus { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public IList<string> Types { get; set; }
    }

    public class Northeast
    {

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Southwest
    {

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Bounds
    {

        [JsonProperty("northeast")]
        public Northeast Northeast { get; set; }

        [JsonProperty("southwest")]
        public Southwest Southwest { get; set; }
    }

    public class DistanceOp
    {

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class Duration
    {

        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public int Value { get; set; }
    }

    public class EndLocation
    {

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class StartLocation
    {

        [JsonProperty("lat")]
        public double Lat { get; set; }

        [JsonProperty("lng")]
        public double Lng { get; set; }
    }

    public class Polyline
    {

        [JsonProperty("points")]
        public string Points { get; set; }
    }

    public class Step
    {

        [JsonProperty("distance")]
        public DistanceOp Distance { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("end_location")]
        public EndLocation EndLocation { get; set; }

        [JsonProperty("html_instructions")]
        public string HtmlInstructions { get; set; }

        [JsonProperty("polyline")]
        public Polyline Polyline { get; set; }

        [JsonProperty("start_location")]
        public StartLocation StartLocation { get; set; }

        [JsonProperty("travel_mode")]
        public string TravelMode { get; set; }

        [JsonProperty("maneuver")]
        public string Maneuver { get; set; }

        [JsonProperty("transit_details")]//test
        public TransitDetails TransitDetails { get; set; }
    }

    public class Leg
    {

        [JsonProperty("distance")]
        public DistanceOp Distance { get; set; }

        [JsonProperty("duration")]
        public Duration Duration { get; set; }

        [JsonProperty("end_address")]
        public string EndAddress { get; set; }

        [JsonProperty("end_location")]
        public EndLocation EndLocation { get; set; }

        [JsonProperty("start_address")]
        public string StartAddress { get; set; }

        [JsonProperty("start_location")]
        public StartLocation StartLocation { get; set; }

        [JsonProperty("steps")]
        public IList<Step> Steps { get; set; }

        [JsonProperty("traffic_speed_entry")]
        public IList<object> TrafficSpeedEntry { get; set; }

        [JsonProperty("via_waypoint")]
        public IList<object> ViaWaypoint { get; set; }
    }

    public class OverviewPolyline
    {

        [JsonProperty("points")]
        public string Points { get; set; }
    }

    public class Route
    {

        [JsonProperty("bounds")]
        public Bounds Bounds { get; set; }

        [JsonProperty("copyrights")]
        public string Copyrights { get; set; }

        [JsonProperty("legs")]
        public IList<Leg> Legs { get; set; }

        [JsonProperty("overview_polyline")]
        public OverviewPolyline OverviewPolyline { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("warnings")]
        public IList<object> Warnings { get; set; }

        [JsonProperty("waypoint_order")]
        public IList<object> WaypointOrder { get; set; }
    }

    public class GoogleDirection
    {

        [JsonProperty("geocoded_waypoints")]
        public IList<GeocodedWaypoint> GeocodedWaypoints { get; set; }

        [JsonProperty("routes")]
        public IList<Route> Routes { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
