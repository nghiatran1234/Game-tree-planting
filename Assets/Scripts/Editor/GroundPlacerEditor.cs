using UnityEngine;
using UnityEditor;  // <-- THÊM DÒNG NÀY (Rất quan trọng)

// Script này chỉ chạy trong Unity Editor để thêm nút vào Inspector
[CustomEditor(typeof(GroundPlacer))]
public class GroundPlacerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Vẽ các biến public mặc định (yOffset, groundLayerMask)
        DrawDefaultInspector();

        // Lấy đối tượng GroundPlacer đang được chọn
        GroundPlacer placer = (GroundPlacer)target;

        // Tạo một nút bấm
        if (GUILayout.Button("Hạ xuống đất (Editor)"))
        {
            // Gọi hàm xử lý riêng cho Editor
            PlaceOnGround_Editor(placer);
        }
    }

    // Hàm này chỉ chạy trong Editor khi nhấn nút
    private void PlaceOnGround_Editor(GroundPlacer placer)
    {
        Transform objectTransform = placer.transform;
        Vector3 origin = objectTransform.position + Vector3.up * 1.0f;
        RaycastHit hit;

        if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, placer.groundLayerMask))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y += placer.yOffset;

            // Ghi lại thay đổi để có thể Undo (Ctrl+Z)
            Undo.RecordObject(objectTransform, "Place Object on Ground");
            objectTransform.position = targetPosition;

            Debug.Log(placer.gameObject.name + " đã được hạ xuống đất (Editor) tại vị trí Y = " + targetPosition.y);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy mặt đất bên dưới đối tượng: " + placer.gameObject.name + " (Editor)");
        }
    }
}