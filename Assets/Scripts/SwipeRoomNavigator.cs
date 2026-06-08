using UnityEngine;
using UnityEngine.Events;

namespace SwipeRoomNavigation
{
    public class SwipeRoomNavigator : MonoBehaviour
    {
        [Header("Camera / Object to move")]
        [Tooltip("Usually your Main Camera. If empty, the script will try Camera.main.")]
        public Transform targetToMove;

        [Header("Room positions")]
        [Tooltip("Put 4 empty GameObjects here - one for each room camera position.")]
        public Transform[] roomPositions;

        [Header("Swipe settings")]
        [Tooltip("Minimum horizontal swipe distance in pixels.")]
        public float minSwipeDistance = 80f;

        [Tooltip("If vertical movement is too big, it will not count as horizontal swipe.")]
        public float maxVerticalSwipe = 120f;

        [Header("Movement settings")]
        public float moveSpeed = 8f;
        public bool useSmoothMovement = true;

        [Header("Optional events")]
        public UnityEvent<int> onRoomChanged;

        private int currentRoomIndex = 0;
        private Vector2 touchStartPosition;
        private Vector2 touchEndPosition;
        private bool isMoving;

        private void Awake()
        {
            if (targetToMove == null && Camera.main != null)
            {
                targetToMove = Camera.main.transform;
            }

            if (roomPositions != null && roomPositions.Length > 0 && targetToMove != null)
            {
                targetToMove.position = roomPositions[currentRoomIndex].position;
            }
        }

        private void Update()
        {
            DetectTouchSwipe();
            DetectMouseSwipeForTestingInEditor();
            MoveToCurrentRoom();
        }

        private void DetectTouchSwipe()
        {
            if (Input.touchCount == 0)
                return;

            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchStartPosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                touchEndPosition = touch.position;
                CheckSwipe(touchStartPosition, touchEndPosition);
            }
        }

        private void DetectMouseSwipeForTestingInEditor()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                touchEndPosition = Input.mousePosition;
                CheckSwipe(touchStartPosition, touchEndPosition);
            }
#endif
        }

        private void CheckSwipe(Vector2 startPosition, Vector2 endPosition)
        {
            if (isMoving)
                return;

            float horizontalDistance = endPosition.x - startPosition.x;
            float verticalDistance = Mathf.Abs(endPosition.y - startPosition.y);

            if (Mathf.Abs(horizontalDistance) < minSwipeDistance)
                return;

            if (verticalDistance > maxVerticalSwipe)
                return;

            if (horizontalDistance < 0)
            {
                GoToNextRoom();
            }
            else
            {
                GoToPreviousRoom();
            }
        }

        public void GoToNextRoom()
        {
            if (roomPositions == null || roomPositions.Length == 0)
                return;

            if (currentRoomIndex < roomPositions.Length - 1)
            {
                currentRoomIndex++;
                isMoving = true;
                onRoomChanged?.Invoke(currentRoomIndex);
            }
        }

        public void GoToPreviousRoom()
        {
            if (roomPositions == null || roomPositions.Length == 0)
                return;

            if (currentRoomIndex > 0)
            {
                currentRoomIndex--;
                isMoving = true;
                onRoomChanged?.Invoke(currentRoomIndex);
            }
        }

        public void GoToRoom(int roomIndex)
        {
            if (roomPositions == null || roomPositions.Length == 0)
                return;

            if (roomIndex < 0 || roomIndex >= roomPositions.Length)
                return;

            currentRoomIndex = roomIndex;
            isMoving = true;
            onRoomChanged?.Invoke(currentRoomIndex);
        }

        private void MoveToCurrentRoom()
        {
            if (!isMoving || targetToMove == null || roomPositions == null || roomPositions.Length == 0)
                return;

            Vector3 targetPosition = roomPositions[currentRoomIndex].position;

            if (useSmoothMovement)
            {
                targetToMove.position = Vector3.Lerp(
                    targetToMove.position,
                    targetPosition,
                    Time.deltaTime * moveSpeed
                );
            }
            else
            {
                targetToMove.position = Vector3.MoveTowards(
                    targetToMove.position,
                    targetPosition,
                    moveSpeed * Time.deltaTime
                );
            }

            if (Vector3.Distance(targetToMove.position, targetPosition) < 0.02f)
            {
                targetToMove.position = targetPosition;
                isMoving = false;
            }
        }
    }
}