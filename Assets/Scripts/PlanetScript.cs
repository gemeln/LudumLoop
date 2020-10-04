﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class PlanetScript : MonoBehaviour
{
    //Member variables
    private bool IsOrbited;
    public BlackHoleScript planet;
    public int mass;
    public Vector3 vi;
    public float G;
    private Vector3 v;
    private Vector2 rocketVelocity;
    private List<BlackHoleDataEntry> orbitingPlanets = new List<BlackHoleDataEntry>();
    private bool[] hasOrbited;
    //Tuple (float score, float degreeStart)
    private GameObject[] planets;

    public int completedOrbits;
    public double test;
    public GameObject orbitCalcPrefab;
    public GameObject orbitCalc;
    void Start()
    {
        v = vi;
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        planets = GameObject.FindGameObjectsWithTag("Planet");
        foreach (GameObject planet in planets)
        {
            orbitingPlanets.Add(new BlackHoleDataEntry(0.0f, transform.position, planet.GetComponent<BlackHoleScript>()));
        }
        // Spawn the orbit calculator for this player
        hasOrbited = new bool[orbitingPlanets.Count];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rocketVelocity = (transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition)).normalized * 20;
        } else {
            rocketVelocity = Vector2.zero;
        }
    }
    private void FixedUpdate()
    {
        Vector3 a = Vector3.zero;
        foreach (BlackHoleDataEntry planet in orbitingPlanets)
        {
           Vector3 r = transform.position - planet.TargetPlanet.transform.position;
           a += -G * planet.TargetPlanet.mass / (r.magnitude * r.magnitude) * r.normalized;
        }
        a += (Vector3) rocketVelocity;
        v += Time.deltaTime * a;
        transform.position = transform.position + v * Time.deltaTime;
        CalculateAngles();
    }

    private void CalculateAngles()
    {
        for(int i = 0; i < orbitingPlanets.Count; i++)
        {
            Vector2 A = transform.position - orbitingPlanets[i].TargetPlanet.transform.position;
            Vector2 B = orbitingPlanets[i].LastPoint - (Vector2) orbitingPlanets[i].TargetPlanet.transform.position;
            float angleChange = -Mathf.Asin(Vector3.Cross(A.normalized,B.normalized).z);
               
            if (!hasOrbited[i] && orbitingPlanets[i].CheckAndUpdate(angleChange,transform.position) ) {
                Debug.Log("1 planet orbit");
                hasOrbited[i] = true;
                if (HandleFullOrbit()) 
                     return;
            }
        }
    }

    //Fully orbited all planets?
    private bool HandleFullOrbit() {
        foreach(bool b in  hasOrbited){
            if (!b) return false;
        }
        hasOrbited = new bool[orbitingPlanets.Count];
        foreach (BlackHoleDataEntry planet in orbitingPlanets) {
            planet.Angle = 0;
            planet.LastPoint = transform.position;
            planet.AngleTargetHigh = Mathf.PI * 2;
            planet.AngleTargetLow = Mathf.PI * -2;
}
        completedOrbits++;
        return true;
    }

    private void OnDestroy()
    {
        Destroy(orbitCalc);
        Debug.Log("Died");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //orbitingPlanets.Add(collision.gameObject.GetComponent<PlanetScript>());
        if (collision.tag.Equals("Planet"))
        {
            Destroy(gameObject);
        } else if (collision.tag.Equals("Star"))
        {
            MainGameScript.Instance.AddStar();
            collision.enabled = false;
            collision.transform.Find("Death").gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //orbitingPlanets.Remove(collision.gameObject.GetComponent<PlanetScript>());
    }

    private double AngleRadians(Vector2 target, Vector2 origin)
    {
        Vector2 vector2 = target - origin;
        Vector2 vector1 = new Vector2(0, 1); // 12 o'clock == 0°, assuming that y goes from bottom to top

        double angleInRadians = Math.Atan2(vector2.y, vector2.x) - Math.Atan2(vector1.y, vector1.x);
        return angleInRadians;
    }
}
