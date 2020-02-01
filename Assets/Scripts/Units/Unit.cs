/*
   Project      : GGJ20-JambonBoursin
   Author		: Yannis Beaux (Kranck), Loic Mathiot (Asterius)
   Date		    : 01 / 02 / 2020
   Description  : Represent a unit
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	//
	// Private
	//
	private UI.UnitUI m_ui;

	private int		m_hp;
	private int		m_maxHp;
	private bool	m_isSelected;

	private void Start()
	{
		m_ui = transform.Find("UnitUI").GetComponent<UI.UnitUI>();

		m_maxHp = 100;

		SetSelected(false);
		SetHP(m_maxHp);
	}

	/// <summary>
	/// Set the selected status of this unit
	/// </summary>
	/// <param name="isSelected"></param>
	public void SetSelected(bool isSelected)
	{
		m_isSelected = isSelected;

		m_ui.SetDisplayed(isSelected);

		//
		// Set the highlight property of the material
		GetComponentInChildren<Renderer>().material.SetInt("_HighLight", isSelected ? 1 : 0);
	}

	/// <summary>
	/// Return true if this unit is selected, false otherwise
	/// </summary>
	/// <returns></returns>
	public bool IsSelected()
	{
		return m_isSelected;
	}

	public void SetHP(int hp)
	{
		if (hp < 0)
		{
			hp = 0;
		}

		if (hp > m_maxHp)
		{
			hp = m_maxHp;
		}

		m_hp = hp;

		m_ui.SetLifeBarFill(m_hp / (float)m_maxHp);
	}

	public int GetHP()
	{
		return m_hp;
	}

	public void Hit(int damage)
	{
		SetHP(m_hp - damage);
	}
}
