using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Text.RegularExpressions;

public class Randomize
{

	private System.Random rnd;
	private Dictionary<string, double> dic;
	private List<string> listA;
	private List<double> listB;
	private int variance;

	//val is the scale of randomization
	public Randomize (int val)
	{
		//Initalize Randomization
		rnd = new System.Random (DateTime.Now.Millisecond);

		//Import numbers
		readInverseBinomialNumbers ();

		//set the scale of randomization
		variance = val;
	}

	//Print function for testing method
//	public void print ()
//	{
//		for (int i = 0; i < listA.Count; i++) {
//			System.Console.Write (listA [i] + "  ");
//			System.Console.WriteLine (listB [i]);
//		}
//	}

	//Read inverse binomial numbers from the csv file
	public void readInverseBinomialNumbers ()
	{
		//two lists used to save two columns of data from csv 
		listA = new List<string> ();
		listB = new List<double> ();

		TextAsset txt = Resources.Load ("InverseNorm") as TextAsset;
		String [] file = Regex.Split (txt.text,"\n|\r|\r\n");

		foreach (String line in file) {
			if (line.Trim ().Equals ("")) {
				continue;
			} else {
				String[] values = line.Split (',');
				if (values [0].Equals ("") || values [1].Equals ("")) {
					continue;
				}
				if (values [0].Trim ().Equals ("percentile", System.StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				} else {
					//add values to the list
					listA.Add (values [0].Trim ());
				}
				if (values [0].Trim ().Equals ("z", System.StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				} else {
					//add values to the list
					listB.Add (Double.Parse (values [1].Trim ()));
				}
			}
		}

		//Add the other half of the binomial table

		txt = Resources.Load ("InverseNorm") as TextAsset;
		file = Regex.Split (txt.text,"\n|\r|\r\n");

		foreach (String line in file) {
			if (line.Trim ().Equals ("")) {
				continue;
			} else {
				String[] values = line.Split (',');
				if (values [0].Equals ("") || values [1].Equals ("")) {
					continue;
				}
				if (values [0].Trim ().Equals ("percentile", System.StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				} else {
					//add values to the list
					listA.Add ((1 - Double.Parse (values [0].Trim ())).ToString ());
				}
				if (values [0].Trim ().Equals ("z", System.StringComparison.InvariantCultureIgnoreCase)) {
					continue;
				} else {
					//add values to the list
					listB.Add (-Double.Parse (values [1].Trim ()));
				}
			}
		}

		//Build the dictionary
		dic = new Dictionary<string, double> ();
		for (int i = 0; i < listA.Count; i++) {
			if (!dic.ContainsKey (listA [i])) {
				dic.Add (listA [i], listB [i]);
			}
		}
	}

	//Testing
//	public void test ()
//	{
//
//		//Initialize String
//		string csv = "";
//		//Header
//		csv += "1, 2, 3, 4, 5, 6, 7 \n";
//
//		for (int i = 0; i < 10000; i++) {
//			//7 inputs
//			csv += curedPopulationRnd (5, 0.6) + ", ";
//			csv += curedPopulationRnd (5, 0.9) + ", ";
//			csv += curedPopulationRnd (15, 0.6) + ", ";
//			csv += curedPopulationRnd (15, 0.9) + ", ";
//			csv += curedPopulationRnd (50, 0.6) + ", ";
//			csv += curedPopulationRnd (50, 0.9) + ", ";
//			csv += curedPopulationRnd (500, 0.9) + "\n "; //Newline
//		}
//		System.IO.File.WriteAllText ("CSVData.csv", csv);
//	}

	//Give number of population N and chance of cure P, return a randomized number of cured population
	public int curedPopulationRnd (double p)
	{
		//Random number generator
		int intRnd = rnd.Next (1, 1999);      //Random number from 1 to 1999
		double numRnd = intRnd / 2000.0;   //Convert to number from 0.0005 to 0.9995
		double zValue = dic [numRnd.ToString ()]; //scale the random number to population with zValue

		//Formula which calculate the value
		double curedPopulation = zValue * variance * Math.Sqrt (p * (1 - p)) + p;

		//Cured people cannot be bigger than the total population
		if (curedPopulation > 1)
			curedPopulation = 1;
		if (curedPopulation < 0)
			curedPopulation = 0;

		return (int) Math.Round(curedPopulation, MidpointRounding.AwayFromZero);
	}




}

