using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Timers;

public class SunScript : MonoBehaviour {

    public readonly int DAYS_OFFSET = 60;
    private readonly double SPEED_MULTIPLIER = 1;

	public Light sun;
	public DateTime currentDateTime;

	public Text sunriseText;
	public Text solarNoonText;
	public Text sunsetText;
	public Text dayLengthText;
	public Text solarElevationText;
	public Text solarAzimuthText;
	public Text latitudeText;
	public Text longitudeText;
	public Text dateTimeText;

	private string sunriseStr;
	private string solarNoonStr;
	private string sunsetStr;
	private string dayLengthStr;
	private string solarElevationStr;
	private string solarAzimuthStr;
	private string latitudeStr;
	private string longitudeStr;
	private string dateTimeStr;

	private double latitude;
	private double longitude;
    private double longitudeNew;

    private bool isCustomTimezone = false;

    private int oldX;
    private int newX;

    private int timeZone = 0;

	private Timer timer;

	// Use this for initialization
	void Start () {

		currentDateTime = DateTime.UtcNow;

		timer = new System.Timers.Timer();
		timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
		timer.Interval = 100 / SPEED_MULTIPLIER;
		timer.Enabled = true;
	}

	private void OnTimedEvent(object source, ElapsedEventArgs e)
	{
		currentDateTime = currentDateTime.AddMinutes (1);
	}
	
	// Update is called once per frame
	void Update () {

        longitudeNew = GameObject.FindGameObjectWithTag("Player").transform.position.x;

        if (longitudeNew < longitude - 0.00001 || longitudeNew > longitude + 0.00001)
        {
            isCustomTimezone = false;
        }

        if (Input.anyKeyDown)
        {
            if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha1))
            {
                timeZone = -1;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha2))
            {
                timeZone = -2;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha3))
            {
                timeZone = -3;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha4))
            {
                timeZone = -4;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha5))
            {
                timeZone = -5;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha6))
            {
                timeZone = -6;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha7))
            {
                timeZone = -7;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha8))
            {
                timeZone = -8;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Alpha9))
            {
                timeZone = -9;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.Z))
            {
                timeZone = -10;
            }
            else if (Input.GetKey(KeyCode.Minus) && Input.GetKeyDown(KeyCode.X))
            {
                timeZone = -11;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                timeZone = 0;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                timeZone = 1;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                timeZone = 2;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                timeZone = 3;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                timeZone = 4;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                timeZone = 5;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6))
            {
                timeZone = 6;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7))
            {
                timeZone = 7;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8))
            {
                timeZone = 8;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9))
            {
                timeZone = 9;
            }
            else if (Input.GetKeyDown(KeyCode.Z))
            {
                timeZone = 10;
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                timeZone = 11;
            }
            else if (Input.GetKeyDown(KeyCode.C))
            {
                timeZone = 12;
            }

            isCustomTimezone = true;
        }

		longitude = GameObject.FindGameObjectWithTag("Player").transform.position.x;
		latitude = GameObject.FindGameObjectWithTag("Player").transform.position.z;

		latitudeStr = latitude.ToString ("0.00");
		latitudeText.text = latitudeStr;

		longitudeStr = longitude.ToString ("0.00");
		longitudeText.text = longitudeStr;

        if (!isCustomTimezone)
        {
            timeZone = GetTimeZone(longitude);
        }

		double[] sunValues = CalculateSun (latitude, longitude, currentDateTime.AddHours(timeZone).AddDays(DAYS_OFFSET), timeZone);

		sunriseStr = convertTimeToProperForm (sunValues [0]);
		sunriseText.text = sunriseStr;

		solarNoonStr = convertTimeToProperForm (sunValues [1]);
		solarNoonText.text = solarNoonStr;

		sunsetStr = convertTimeToProperForm (sunValues [2]);
		sunsetText.text = sunsetStr;

		dayLengthStr = convertTimeToProperForm (sunValues [3] / 1440);
		dayLengthText.text = dayLengthStr;

		solarElevationStr = String.Format ("{0:0.0}", sunValues [4]);
		solarElevationText.text = solarElevationStr;

		solarAzimuthStr = String.Format ("{0:0.0}", sunValues [5]);
		solarAzimuthText.text = solarAzimuthStr;

		dateTimeStr = currentDateTime.AddHours(timeZone).AddDays(DAYS_OFFSET).ToString ("dd/MM/yyyy HH:mm:ss") + " UTC " + timeZone;
		dateTimeText.text = dateTimeStr;

		transform.rotation = Quaternion.Euler((float)sunValues[4], (float)sunValues[5] + 180, 0);

	}

	double[] CalculateSun(double lat, double longi, DateTime dateTime, double timeZone)
	{

		double E = (3600 * dateTime.Hour + 60 * dateTime.Minute + dateTime.Second) / 86400.0;
		double F = ToJulianDate(dateTime);
		double G = (F - 2451545) / 36525; //Julian century
		double I = (280.46646 + G * (36000.76983 + G * 0.0003032)) % 360; //Geom Mean Long Sun (deg)
		double J = 357.52911 + G * (35999.05029 - 0.0001537 * G); //Geom mean anom sun (deg)
		double K = 0.016708634 - G * (0.000042037 + 0.0000001267 * G); //Eccent earth orbit
		double L = Math.Sin(J * 0.0174533) * (1.914602 - G * (0.004817 + 0.000014 * G)) + Math.Sin(2 * J * 0.0174533) * (0.019993 - 0.000101 * G) + Math.Sin(3 * J * 0.0174533) * 0.000289; //Sun equation of center
		double M = I + L; //Sun true long (deg)
		double N = J + L; //Sun true anom (deg)
		double O = (1.000001018 * (1 - K * K)) / (1 + K * Math.Cos(N * 0.0174533)); //Sun rad vector (AUs)
		double P = M - 0.00569 - 0.00478 * Math.Sin((125.04 - 1934.136 * G) * 0.0174533); //Sun app long (deg)
		double Q = 23 + (26 + ((21.448 - G * (46.815 + G * (0.00059 - G * 0.001813)))) / 60) / 60; //Mean obliq ecliptic (deg)
		double R = Q + 0.00256 * Math.Cos((125.04 - 1934.136 * G) * 0.0174533); //Obliq corr (deg)
		double S = (Math.Atan2(Math.Cos(P * 0.0174533), Math.Cos(Q * 0.0174533) * Math.Sin(P * 0.0174533))) * 57.2958; //Sun rate of ascent(deg)
		double T = (Math.Asin(Math.Sin(R * 0.0174533) * Math.Sin(P * 0.0174533))) * 57.2958; //Sun declination (deg)
		double U = Math.Tan(R / 2 * 0.0174533) * Math.Tan(R / 2 * 0.0174533); //var y
		double V = 4 * (U * Math.Sin(2 * I * 0.0174533) - 2 * K * Math.Sin(J * 0.0174533) + 4 * K * U * Math.Sin(J * 0.0174533) * Math.Cos(2 * I * 0.0174533) - 0.5 * U * U * Math.Sin(4 * I * 0.0174533) - 1.25 * K * K * Math.Sin(2 * J * 0.0174533)) * 57.2958; //Equation of time (minutes)
		double W = (Math.Acos(Math.Cos(90.833 * 0.0174533) / (Math.Cos(lat * 0.0174533) * Math.Cos(T * 0.0174533)) - Math.Tan(lat * 0.0174533) * Math.Tan(T * 0.0173433))) * 57.2958; //HA Sunrise (deg)
		double X = (720 - 4 * longi - V + timeZone * 60) / 1440; //Solar noon
		double Y = X - W * 4 / 1440; //Sunrise
		double Z = X + W * 4 / 1440; //Sunset
		double AA = 8 * W; //Day length (minutes)
		double AB = (E * 1440 + V + 4 * longi - 60 * timeZone) % 1440; //True solar time
		double AC = 0.0; //Hour angle(deg)
		if(AB / 4 < 0)
		{
			AC = AB / 4 + 180;
		}
		else{
			AC = AB / 4 - 180;
		}
		double AD = (Math.Acos(Math.Sin(lat * 0.0174533) * Math.Sin(T * 0.0174533) + Math.Cos(lat * 0.0174533) * Math.Cos(T * 0.0174533) * Math.Cos(AC * 0.0174533))) * 57.2958; //Solar zenith angle(deg)
		double AE = 90 - AD; //Solar elevation angle(deg)
		double AH = 0.0; //Solar azimuth angle (clockwise from North)
		if(AC > 0)
		{
			AH = ((Math.Acos(((Math.Sin(lat * 0.0174533) * Math.Cos(AD * 0.0174533)) - Math.Sin(T * 0.0174533)) / (Math.Cos(lat * 0.0174533) * Math.Sin(AD * 0.0174533)))) * 57.2958 + 180) % 360;
		}
		else
		{
			AH = (540 - (Math.Acos(((Math.Sin(lat * 0.0174533) * Math.Cos(AD * 0.0174533)) - Math.Sin(T * 0.0174533)) / (Math.Cos(lat * 0.0174533) * Math.Sin(AD * 0.0174533)))) * 57.2958) % 360;
		}
			
		double[] returnValue = {Y, X, Z, AA, AE, AH};
		return returnValue;

	}

	double ToJulianDate(DateTime date)
	{
		return date.ToOADate() + 2415018.5;
	}

	string convertTimeToProperForm(double timeIn){

		int time = (int)(timeIn * 86400); //convert to seconds
		int hour = (int)(time / 3600);
		int minute = (int) ((time - (double) (hour) * 3600) / 60);
		int second = (int) (time - (double) (hour) * 3600 - (double) (minute) * 60);

		return hour.ToString("00") + ":" + minute.ToString("00") + ":" + second.ToString("00");

	}

	int GetTimeZone(double longitude){

		double a = longitude + 195;
		if (a > 360)
			a -= 360;

		int b = (int)(a / 15.0);
		return b - 12;

	}

}
