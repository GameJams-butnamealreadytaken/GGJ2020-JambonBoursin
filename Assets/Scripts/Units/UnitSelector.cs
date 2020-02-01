/*
   Project      : GGJ20-JambonBoursin
   Author		: Yannis Beaux (Kranck)
   Date		    : 31 / 01 / 2020
   Description  : Unit Selector
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class UnitSelector : MonoBehaviour
{
	[Header("Configuration")]
	[Tooltip("The tag of the units (only the game objects with this tag will be selectable)")]
	string m_unitTag;   //< The tag that will be checked for gameobjects that are selectable

	[Header("Other")]
	[Tooltip("The image that is used to display the selection rect")]
	public Image m_selectionRectImage;

	//
	// Private
	//
	private bool m_isSelecting;		//< Indicates if the user is currently selecting 
	private Vector2 m_mouseStartPosition;   //< The current selection start position for the current selection

	private List<Unit> m_selectedUnits = new List<Unit>();

    void Start()
    {
		//
		// We hide the selection rect image
		m_selectionRectImage.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
		//
		// Simple select when mouse up (only if we are not multiple selecting
		if (Input.GetMouseButtonUp(0) && !m_isSelecting)
		{
			//
			// Create a ray from the position of the mouse and the camera
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			//
			// Raycast from the position of the mouse 
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				//
				// Check it is a unit
				if (hitInfo.collider.GetComponent<Unit>() != null)
				{
					//
					// We reset the selection
					ResetSelection();

					//
					// Select this unit
					hitInfo.collider.GetComponent<Unit>().SetSelected(true);
					m_selectedUnits.Add(hitInfo.collider.GetComponent<Unit>());
				}
			}
		}

		//
		// Check if the mouse is pressed
		if (Input.GetMouseButton(0))
		{
			//
			// We check if the mouse is moving
			if (Mouse.current.delta.x.ReadValue() != 0f && Mouse.current.delta.y.ReadValue() != 0f)
			{
				//
				// We store the mouse position if it is the first frame we are selecting
				if (!m_isSelecting)
				{
					//
					// Store the mouse position
					m_mouseStartPosition = Input.mousePosition;

					//
					// We start the selection, so we clear the array of selected units
					ResetSelection();
				}

				//
				// We are selecting
				m_isSelecting = true;
			}
		}
		else
		{
			//
			// We are selecting 
			m_isSelecting = false;

			//
			// Hide the image
			m_selectionRectImage.enabled = false;
		}

		//
		// We are selecting
		if (m_isSelecting)
		{
			//
			// We display the selection rect
			m_selectionRectImage.enabled = true;

			//
			// We retrieve the position of the mouse
			Vector2 currentMousePosition = Input.mousePosition;

			//
			// We set the position of the selection rect to the current position of the mouse
			m_selectionRectImage.transform.position = m_mouseStartPosition;

			//
			// Compute the height of the selection rect 
			float width = m_mouseStartPosition.x - currentMousePosition.x;
			float height = m_mouseStartPosition.y - currentMousePosition.y;

			//
			// We set the scale of the image rect
			m_selectionRectImage.transform.localScale = new Vector3(width > 0f ? -1f : 1f, height > 0f ? 1f : -1f, 1f);

			//
			// Set the size of the rect
			m_selectionRectImage.rectTransform.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));

			//
			// Convert the mouse start and current position to viewport positions
			Vector3 startViewportPosition = Camera.main.ScreenToViewportPoint(m_mouseStartPosition);
			Vector3 endViewportPosition = Camera.main.ScreenToViewportPoint(currentMousePosition);

			//
			// Retrieve the min and maximum positions to create bounds
			// REF : https://hyunkell.com/blog/rts-style-unit-selection-in-unity-5/
			Vector3 min = Vector3.Min(startViewportPosition, endViewportPosition);
			Vector3 max = Vector3.Max(startViewportPosition, endViewportPosition);
			min.z = Camera.main.nearClipPlane;
			max.z = Camera.main.farClipPlane;

			//
			//Create the bounds object
			Bounds bounds = new Bounds();
			bounds.SetMinMax(min, max);

			//
			// Iterate over the units
			foreach (var item in UnitsManager.m_instance.GetUnits())
			{
				//
				// We check if the bound contains the current item 
				if (bounds.Contains(Camera.main.WorldToViewportPoint(item.transform.position)))
				{
					//
					// The current item is selected, add it in the array
					m_selectedUnits.Add(item as Unit);
					item.SetSelected(true);
				}
				else
				{
					m_selectedUnits.Remove(item as Unit);
				}
			}

		}
	}

	private void ResetSelection()
	{
		foreach (Unit item in m_selectedUnits)
		{
			item.SetSelected(false);
		}
	}
}
