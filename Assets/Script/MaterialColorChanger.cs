using UnityEngine;
using UnityEngine.UI;

public class MaterialColorChanger : MonoBehaviour
{
    [Header("Target")]
    [Tooltip("La RawImage su cui applicare il materiale")]
    public RawImage targetImage;

    [Header("Sezione 1")]
    public Material materiale_S1_01;
    public Material materiale_S1_02;
    public Material materiale_S1_03;
    public Material materiale_S1_04;
    public Material materiale_S1_05;

    [Header("Sezione 2")]
    public Material materiale_S2_01;
    public Material materiale_S2_02;
    public Material materiale_S2_03;
    public Material materiale_S2_04;
    public Material materiale_S2_05;

    [Header("Sezione 3")]
    public Material materiale_S3_01;
    public Material materiale_S3_02;
    public Material materiale_S3_03;
    public Material materiale_S3_04;
    public Material materiale_S3_05;

    private void ApplicaMateriale(Material mat)
    {
        if (targetImage == null)
        {
            Debug.LogWarning("[MaterialColorChanger] targetImage non assegnata!");
            return;
        }
        if (mat == null)
        {
            Debug.LogWarning("[MaterialColorChanger] Materiale non assegnato!");
            return;
        }
        targetImage.material = mat;
    }

    public void ApplicaS1_01() => ApplicaMateriale(materiale_S1_01);
    public void ApplicaS1_02() => ApplicaMateriale(materiale_S1_02);
    public void ApplicaS1_03() => ApplicaMateriale(materiale_S1_03);
    public void ApplicaS1_04() => ApplicaMateriale(materiale_S1_04);
    public void ApplicaS1_05() => ApplicaMateriale(materiale_S1_05);

    public void ApplicaS2_01() => ApplicaMateriale(materiale_S2_01);
    public void ApplicaS2_02() => ApplicaMateriale(materiale_S2_02);
    public void ApplicaS2_03() => ApplicaMateriale(materiale_S2_03);
    public void ApplicaS2_04() => ApplicaMateriale(materiale_S2_04);
    public void ApplicaS2_05() => ApplicaMateriale(materiale_S2_05);

    public void ApplicaS3_01() => ApplicaMateriale(materiale_S3_01);
    public void ApplicaS3_02() => ApplicaMateriale(materiale_S3_02);
    public void ApplicaS3_03() => ApplicaMateriale(materiale_S3_03);
    public void ApplicaS3_04() => ApplicaMateriale(materiale_S3_04);
    public void ApplicaS3_05() => ApplicaMateriale(materiale_S3_05);
}
