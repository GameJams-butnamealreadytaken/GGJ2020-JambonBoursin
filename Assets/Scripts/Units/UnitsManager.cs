/*
   Project      : GGJ20-JambonBoursin
   Author		: Yannis Beaux (Kranck)
   Date		    : 01 / 02 / 2020
   Description  : Manager for all the units
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
	public List<Unit> m_units = new List<Unit>();

	public static UnitsManager m_instance;

	public void Awake()
	{
		if (m_instance != this && m_instance != null)
		{
			Destroy(gameObject);
		}
		m_instance = this;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public List<Unit> GetUnits()
	{
		return m_units;
	}
}
