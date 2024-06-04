//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.6.3
//     from Assets/PlayerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerControls: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""a0bb67eb-2edf-4e29-9b42-60df6752e1fc"",
            ""actions"": [
                {
                    ""name"": ""MoveForward"",
                    ""type"": ""Button"",
                    ""id"": ""3881e936-248b-449c-b7ba-abdec05e1403"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveBack"",
                    ""type"": ""Button"",
                    ""id"": ""ebcd1bb1-0803-44d6-8e17-55329d81fb86"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveLeft"",
                    ""type"": ""Button"",
                    ""id"": ""c5043317-1171-4454-abc4-29385afa6fa2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveRight"",
                    ""type"": ""Button"",
                    ""id"": ""b2ab33eb-8ad5-47ea-acb7-3a7168f8fea7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""SubmitPath"",
                    ""type"": ""Button"",
                    ""id"": ""d631599c-b054-4606-b8db-d6832ba53256"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3fe1d92c-a95a-4762-8cd7-92b30329693d"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5cc13377-5264-4882-b6e1-d92298d9e14e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4cf37539-be2f-44b4-8949-b6a8489e7503"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b59c2b17-9d15-4fac-9605-52ed43fca39b"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1100f327-8fe6-4ad4-baf7-0e639d5612e1"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb6382d4-24a2-4620-a64b-ba0718f5416b"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""037e9768-d19b-4200-9757-5f2196c83099"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62541446-6ed2-48c0-89c6-c51b77959b26"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": ""Tap"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8cde25f6-07b9-4984-9ce5-094c216c907f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SubmitPath"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""82b921d3-2970-4518-ab1b-a8b19dabe14b"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SubmitPath"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Basic"",
            ""id"": ""7cb40105-a397-49be-9044-a29a211fad31"",
            ""actions"": [
                {
                    ""name"": ""Escape"",
                    ""type"": ""Button"",
                    ""id"": ""bee9967e-508f-4583-81e7-3078019a9ef7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Submit"",
                    ""type"": ""Button"",
                    ""id"": ""1889ebcc-ffbc-405f-99ea-3e0f02345112"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Tab"",
                    ""type"": ""Button"",
                    ""id"": ""0c046eba-4774-4bb3-b7ec-963de4a91317"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Skip"",
                    ""type"": ""Button"",
                    ""id"": ""85720b7e-de8d-47cb-a963-10ef369efbdf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Hold"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""3ac4950c-56d5-495f-8e61-9d81c85a87db"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d6ef31c2-5713-4ce0-b843-8cf740d0c20f"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Escape"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1f6128d6-0d73-46b4-8cbc-a9ebb267ed46"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8203bb25-a032-420f-803d-49afe4244b9a"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Submit"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a04c61d-733d-429c-be46-a816a0832e5e"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Tab"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6540d222-e270-4049-a0ee-e741852c9d6c"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ad59bf82-b2df-41ba-90a2-e68ccdf2f9b7"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Skip"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""3e170636-b612-4311-8eb9-e26d512c2128"",
            ""actions"": [
                {
                    ""name"": ""RotateLeft"",
                    ""type"": ""Button"",
                    ""id"": ""68ea20fe-53e2-43ed-8da2-9bd97ca5d345"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""RotateRight"",
                    ""type"": ""Button"",
                    ""id"": ""01e26056-1cba-4706-b182-667316285ac7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""409cc842-df91-479d-b6da-18fb5388fc1e"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""771d1d4d-41c5-4961-ad17-c53e4358eb3d"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""28b97dbb-a165-40f2-b641-b18f8a98344f"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eeecd6f6-7245-4dc7-9b10-b36f97622ff7"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RotateRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""ArcadeMovement"",
            ""id"": ""2cbb1bb1-7f6f-4b45-bd09-1c64787399f2"",
            ""actions"": [
                {
                    ""name"": ""MoveForward"",
                    ""type"": ""Button"",
                    ""id"": ""bd0ad2fc-f42a-4866-884f-ca92905d4644"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveBack"",
                    ""type"": ""Button"",
                    ""id"": ""c2dec95c-56ac-4996-880c-e82c560e59af"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveLeft"",
                    ""type"": ""Button"",
                    ""id"": ""09a88c42-32a2-4066-82bf-dab8fde08821"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""MoveRight"",
                    ""type"": ""Button"",
                    ""id"": ""2deb7690-6134-4a53-a2c1-b7eac48248df"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Tap"",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""304c310a-be2b-4097-ab9d-e4d0541bbe1b"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveForward"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9046ca1f-ac46-4622-919c-9e659ef76d89"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a13fa4ac-7dc5-4cb0-9bb8-f8f09e9e288e"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0cfc82d9-d225-4499-9c06-7c4e58716e3f"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_MoveForward = m_Movement.FindAction("MoveForward", throwIfNotFound: true);
        m_Movement_MoveBack = m_Movement.FindAction("MoveBack", throwIfNotFound: true);
        m_Movement_MoveLeft = m_Movement.FindAction("MoveLeft", throwIfNotFound: true);
        m_Movement_MoveRight = m_Movement.FindAction("MoveRight", throwIfNotFound: true);
        m_Movement_SubmitPath = m_Movement.FindAction("SubmitPath", throwIfNotFound: true);
        // Basic
        m_Basic = asset.FindActionMap("Basic", throwIfNotFound: true);
        m_Basic_Escape = m_Basic.FindAction("Escape", throwIfNotFound: true);
        m_Basic_Submit = m_Basic.FindAction("Submit", throwIfNotFound: true);
        m_Basic_Tab = m_Basic.FindAction("Tab", throwIfNotFound: true);
        m_Basic_Skip = m_Basic.FindAction("Skip", throwIfNotFound: true);
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_RotateLeft = m_Camera.FindAction("RotateLeft", throwIfNotFound: true);
        m_Camera_RotateRight = m_Camera.FindAction("RotateRight", throwIfNotFound: true);
        // ArcadeMovement
        m_ArcadeMovement = asset.FindActionMap("ArcadeMovement", throwIfNotFound: true);
        m_ArcadeMovement_MoveForward = m_ArcadeMovement.FindAction("MoveForward", throwIfNotFound: true);
        m_ArcadeMovement_MoveBack = m_ArcadeMovement.FindAction("MoveBack", throwIfNotFound: true);
        m_ArcadeMovement_MoveLeft = m_ArcadeMovement.FindAction("MoveLeft", throwIfNotFound: true);
        m_ArcadeMovement_MoveRight = m_ArcadeMovement.FindAction("MoveRight", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Movement
    private readonly InputActionMap m_Movement;
    private List<IMovementActions> m_MovementActionsCallbackInterfaces = new List<IMovementActions>();
    private readonly InputAction m_Movement_MoveForward;
    private readonly InputAction m_Movement_MoveBack;
    private readonly InputAction m_Movement_MoveLeft;
    private readonly InputAction m_Movement_MoveRight;
    private readonly InputAction m_Movement_SubmitPath;
    public struct MovementActions
    {
        private @PlayerControls m_Wrapper;
        public MovementActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveForward => m_Wrapper.m_Movement_MoveForward;
        public InputAction @MoveBack => m_Wrapper.m_Movement_MoveBack;
        public InputAction @MoveLeft => m_Wrapper.m_Movement_MoveLeft;
        public InputAction @MoveRight => m_Wrapper.m_Movement_MoveRight;
        public InputAction @SubmitPath => m_Wrapper.m_Movement_SubmitPath;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void AddCallbacks(IMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_MovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_MovementActionsCallbackInterfaces.Add(instance);
            @MoveForward.started += instance.OnMoveForward;
            @MoveForward.performed += instance.OnMoveForward;
            @MoveForward.canceled += instance.OnMoveForward;
            @MoveBack.started += instance.OnMoveBack;
            @MoveBack.performed += instance.OnMoveBack;
            @MoveBack.canceled += instance.OnMoveBack;
            @MoveLeft.started += instance.OnMoveLeft;
            @MoveLeft.performed += instance.OnMoveLeft;
            @MoveLeft.canceled += instance.OnMoveLeft;
            @MoveRight.started += instance.OnMoveRight;
            @MoveRight.performed += instance.OnMoveRight;
            @MoveRight.canceled += instance.OnMoveRight;
            @SubmitPath.started += instance.OnSubmitPath;
            @SubmitPath.performed += instance.OnSubmitPath;
            @SubmitPath.canceled += instance.OnSubmitPath;
        }

        private void UnregisterCallbacks(IMovementActions instance)
        {
            @MoveForward.started -= instance.OnMoveForward;
            @MoveForward.performed -= instance.OnMoveForward;
            @MoveForward.canceled -= instance.OnMoveForward;
            @MoveBack.started -= instance.OnMoveBack;
            @MoveBack.performed -= instance.OnMoveBack;
            @MoveBack.canceled -= instance.OnMoveBack;
            @MoveLeft.started -= instance.OnMoveLeft;
            @MoveLeft.performed -= instance.OnMoveLeft;
            @MoveLeft.canceled -= instance.OnMoveLeft;
            @MoveRight.started -= instance.OnMoveRight;
            @MoveRight.performed -= instance.OnMoveRight;
            @MoveRight.canceled -= instance.OnMoveRight;
            @SubmitPath.started -= instance.OnSubmitPath;
            @SubmitPath.performed -= instance.OnSubmitPath;
            @SubmitPath.canceled -= instance.OnSubmitPath;
        }

        public void RemoveCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_MovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_MovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public MovementActions @Movement => new MovementActions(this);

    // Basic
    private readonly InputActionMap m_Basic;
    private List<IBasicActions> m_BasicActionsCallbackInterfaces = new List<IBasicActions>();
    private readonly InputAction m_Basic_Escape;
    private readonly InputAction m_Basic_Submit;
    private readonly InputAction m_Basic_Tab;
    private readonly InputAction m_Basic_Skip;
    public struct BasicActions
    {
        private @PlayerControls m_Wrapper;
        public BasicActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Escape => m_Wrapper.m_Basic_Escape;
        public InputAction @Submit => m_Wrapper.m_Basic_Submit;
        public InputAction @Tab => m_Wrapper.m_Basic_Tab;
        public InputAction @Skip => m_Wrapper.m_Basic_Skip;
        public InputActionMap Get() { return m_Wrapper.m_Basic; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BasicActions set) { return set.Get(); }
        public void AddCallbacks(IBasicActions instance)
        {
            if (instance == null || m_Wrapper.m_BasicActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_BasicActionsCallbackInterfaces.Add(instance);
            @Escape.started += instance.OnEscape;
            @Escape.performed += instance.OnEscape;
            @Escape.canceled += instance.OnEscape;
            @Submit.started += instance.OnSubmit;
            @Submit.performed += instance.OnSubmit;
            @Submit.canceled += instance.OnSubmit;
            @Tab.started += instance.OnTab;
            @Tab.performed += instance.OnTab;
            @Tab.canceled += instance.OnTab;
            @Skip.started += instance.OnSkip;
            @Skip.performed += instance.OnSkip;
            @Skip.canceled += instance.OnSkip;
        }

        private void UnregisterCallbacks(IBasicActions instance)
        {
            @Escape.started -= instance.OnEscape;
            @Escape.performed -= instance.OnEscape;
            @Escape.canceled -= instance.OnEscape;
            @Submit.started -= instance.OnSubmit;
            @Submit.performed -= instance.OnSubmit;
            @Submit.canceled -= instance.OnSubmit;
            @Tab.started -= instance.OnTab;
            @Tab.performed -= instance.OnTab;
            @Tab.canceled -= instance.OnTab;
            @Skip.started -= instance.OnSkip;
            @Skip.performed -= instance.OnSkip;
            @Skip.canceled -= instance.OnSkip;
        }

        public void RemoveCallbacks(IBasicActions instance)
        {
            if (m_Wrapper.m_BasicActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IBasicActions instance)
        {
            foreach (var item in m_Wrapper.m_BasicActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_BasicActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public BasicActions @Basic => new BasicActions(this);

    // Camera
    private readonly InputActionMap m_Camera;
    private List<ICameraActions> m_CameraActionsCallbackInterfaces = new List<ICameraActions>();
    private readonly InputAction m_Camera_RotateLeft;
    private readonly InputAction m_Camera_RotateRight;
    public struct CameraActions
    {
        private @PlayerControls m_Wrapper;
        public CameraActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @RotateLeft => m_Wrapper.m_Camera_RotateLeft;
        public InputAction @RotateRight => m_Wrapper.m_Camera_RotateRight;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void AddCallbacks(ICameraActions instance)
        {
            if (instance == null || m_Wrapper.m_CameraActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_CameraActionsCallbackInterfaces.Add(instance);
            @RotateLeft.started += instance.OnRotateLeft;
            @RotateLeft.performed += instance.OnRotateLeft;
            @RotateLeft.canceled += instance.OnRotateLeft;
            @RotateRight.started += instance.OnRotateRight;
            @RotateRight.performed += instance.OnRotateRight;
            @RotateRight.canceled += instance.OnRotateRight;
        }

        private void UnregisterCallbacks(ICameraActions instance)
        {
            @RotateLeft.started -= instance.OnRotateLeft;
            @RotateLeft.performed -= instance.OnRotateLeft;
            @RotateLeft.canceled -= instance.OnRotateLeft;
            @RotateRight.started -= instance.OnRotateRight;
            @RotateRight.performed -= instance.OnRotateRight;
            @RotateRight.canceled -= instance.OnRotateRight;
        }

        public void RemoveCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(ICameraActions instance)
        {
            foreach (var item in m_Wrapper.m_CameraActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_CameraActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public CameraActions @Camera => new CameraActions(this);

    // ArcadeMovement
    private readonly InputActionMap m_ArcadeMovement;
    private List<IArcadeMovementActions> m_ArcadeMovementActionsCallbackInterfaces = new List<IArcadeMovementActions>();
    private readonly InputAction m_ArcadeMovement_MoveForward;
    private readonly InputAction m_ArcadeMovement_MoveBack;
    private readonly InputAction m_ArcadeMovement_MoveLeft;
    private readonly InputAction m_ArcadeMovement_MoveRight;
    public struct ArcadeMovementActions
    {
        private @PlayerControls m_Wrapper;
        public ArcadeMovementActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveForward => m_Wrapper.m_ArcadeMovement_MoveForward;
        public InputAction @MoveBack => m_Wrapper.m_ArcadeMovement_MoveBack;
        public InputAction @MoveLeft => m_Wrapper.m_ArcadeMovement_MoveLeft;
        public InputAction @MoveRight => m_Wrapper.m_ArcadeMovement_MoveRight;
        public InputActionMap Get() { return m_Wrapper.m_ArcadeMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ArcadeMovementActions set) { return set.Get(); }
        public void AddCallbacks(IArcadeMovementActions instance)
        {
            if (instance == null || m_Wrapper.m_ArcadeMovementActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_ArcadeMovementActionsCallbackInterfaces.Add(instance);
            @MoveForward.started += instance.OnMoveForward;
            @MoveForward.performed += instance.OnMoveForward;
            @MoveForward.canceled += instance.OnMoveForward;
            @MoveBack.started += instance.OnMoveBack;
            @MoveBack.performed += instance.OnMoveBack;
            @MoveBack.canceled += instance.OnMoveBack;
            @MoveLeft.started += instance.OnMoveLeft;
            @MoveLeft.performed += instance.OnMoveLeft;
            @MoveLeft.canceled += instance.OnMoveLeft;
            @MoveRight.started += instance.OnMoveRight;
            @MoveRight.performed += instance.OnMoveRight;
            @MoveRight.canceled += instance.OnMoveRight;
        }

        private void UnregisterCallbacks(IArcadeMovementActions instance)
        {
            @MoveForward.started -= instance.OnMoveForward;
            @MoveForward.performed -= instance.OnMoveForward;
            @MoveForward.canceled -= instance.OnMoveForward;
            @MoveBack.started -= instance.OnMoveBack;
            @MoveBack.performed -= instance.OnMoveBack;
            @MoveBack.canceled -= instance.OnMoveBack;
            @MoveLeft.started -= instance.OnMoveLeft;
            @MoveLeft.performed -= instance.OnMoveLeft;
            @MoveLeft.canceled -= instance.OnMoveLeft;
            @MoveRight.started -= instance.OnMoveRight;
            @MoveRight.performed -= instance.OnMoveRight;
            @MoveRight.canceled -= instance.OnMoveRight;
        }

        public void RemoveCallbacks(IArcadeMovementActions instance)
        {
            if (m_Wrapper.m_ArcadeMovementActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IArcadeMovementActions instance)
        {
            foreach (var item in m_Wrapper.m_ArcadeMovementActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_ArcadeMovementActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public ArcadeMovementActions @ArcadeMovement => new ArcadeMovementActions(this);
    public interface IMovementActions
    {
        void OnMoveForward(InputAction.CallbackContext context);
        void OnMoveBack(InputAction.CallbackContext context);
        void OnMoveLeft(InputAction.CallbackContext context);
        void OnMoveRight(InputAction.CallbackContext context);
        void OnSubmitPath(InputAction.CallbackContext context);
    }
    public interface IBasicActions
    {
        void OnEscape(InputAction.CallbackContext context);
        void OnSubmit(InputAction.CallbackContext context);
        void OnTab(InputAction.CallbackContext context);
        void OnSkip(InputAction.CallbackContext context);
    }
    public interface ICameraActions
    {
        void OnRotateLeft(InputAction.CallbackContext context);
        void OnRotateRight(InputAction.CallbackContext context);
    }
    public interface IArcadeMovementActions
    {
        void OnMoveForward(InputAction.CallbackContext context);
        void OnMoveBack(InputAction.CallbackContext context);
        void OnMoveLeft(InputAction.CallbackContext context);
        void OnMoveRight(InputAction.CallbackContext context);
    }
}
