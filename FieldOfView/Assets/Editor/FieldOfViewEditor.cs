using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public sealed class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        var fieldOfView = target as FieldOfView;

        Handles.color = Color.white;

        Handles.DrawWireArc(fieldOfView.transform.position, Vector3.up, Vector3.forward, 360f, fieldOfView.viewRadius);

        var dirAngleA = fieldOfView.DirFromAngle(-fieldOfView.viewAngle / 2f);
        var dirAngleB = fieldOfView.DirFromAngle(fieldOfView.viewAngle / 2f);

        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + dirAngleA * fieldOfView.viewRadius);
        Handles.DrawLine(fieldOfView.transform.position, fieldOfView.transform.position + dirAngleB * fieldOfView.viewRadius);

        Handles.color = Color.red;

        int visibleTargetCount = fieldOfView.visibleTargets != null ? fieldOfView.visibleTargets.Count : 0;

        for (int i = 0; i < visibleTargetCount; i++)
        {
            Handles.DrawLine(fieldOfView.transform.position, fieldOfView.visibleTargets[i].position);
        }
    }
}
