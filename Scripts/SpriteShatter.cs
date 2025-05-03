using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteShatter : MonoBehaviour
{
    public int fragments = 12;                  // Number of triangular pieces
    public float radiusJitter = 0.1f;           // Random variation in fragment shape
    public Material fragmentMaterial;           // Must use the same texture as the sprite

    private void Start()
    {
        Shatter();
    }

    void Shatter()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;
        Texture2D texture = sprite.texture;

        if (fragmentMaterial == null)
        {
            Debug.LogError("No fragment material assigned.");
            return;
        }

        if (!texture.isReadable)
        {
            Debug.LogError("Sprite texture must be Read/Write Enabled in the import settings.");
            return;
        }

        Vector2 size = sprite.bounds.size;
        float maxRadius = Mathf.Max(size.x, size.y) * 0.5f;
        Vector3 center = transform.position;

        for (int i = 0; i < fragments; i++)
        {
            float angleStep = 360f / fragments;
            float angle1 = angleStep * i;
            float angle2 = angleStep * (i + 1);

            float radius1 = maxRadius * (1f + Random.Range(-radiusJitter, radiusJitter));
            float radius2 = maxRadius * (1f + Random.Range(-radiusJitter, radiusJitter));

            Vector2 p1 = Vector2.zero;
            Vector2 p2 = new Vector2(Mathf.Cos(angle1 * Mathf.Deg2Rad), Mathf.Sin(angle1 * Mathf.Deg2Rad)) * radius1;
            Vector2 p3 = new Vector2(Mathf.Cos(angle2 * Mathf.Deg2Rad), Mathf.Sin(angle2 * Mathf.Deg2Rad)) * radius2;

            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] { p1, p2, p3 };
            mesh.triangles = new int[] { 0, 1, 2 };

            // Convert to UV coordinates for texture mapping
            Vector2 uv1 = WorldToUV(center + new Vector3(p1.x, p1.y, 0), sprite, sr);
            Vector2 uv2 = WorldToUV(center + new Vector3(p2.x, p2.y, 0), sprite, sr);
            Vector2 uv3 = WorldToUV(center + new Vector3(p3.x, p3.y, 0), sprite, sr);
            mesh.uv = new Vector2[] { uv1, uv2, uv3 };

            // Create visual-only fragment
            GameObject frag = new GameObject("Fragment");
            frag.transform.position = center;
            frag.transform.localScale = transform.localScale;

            MeshRenderer mr = frag.AddComponent<MeshRenderer>();
            MeshFilter mf = frag.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            mr.material = fragmentMaterial;
        }

        // Hide or destroy the original sprite
        Destroy(gameObject);
    }

    // Converts a world position to a UV coordinate relative to the sprite
    Vector2 WorldToUV(Vector3 worldPos, Sprite sprite, SpriteRenderer sr)
    {
        Vector2 localPos = transform.InverseTransformPoint(worldPos);
        Rect rect = sprite.rect;
        Vector2 uv = (new Vector2(localPos.x, localPos.y) + new Vector2(sprite.bounds.extents.x, sprite.bounds.extents.y)) 
             / new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
        return uv;
    }
}
