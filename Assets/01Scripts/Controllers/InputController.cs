using Rollic;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    private Camera _mainCam;

    public Action Tap;

    public void Initialize(Camera mainCam)
    {
        _mainCam = mainCam;
        playerInput.currentActionMap.FindAction(Constants.InputActions.Tap).performed += OnTap;
    }

    private void OnTap(InputAction.CallbackContext context)
    {
        Ray ray = _mainCam.ScreenPointToRay(context.ReadValue<Vector2>());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            hit.collider.GetComponent<IInteractable>()?.GetInteracted();
        }
        Tap?.Invoke();
    }
}
