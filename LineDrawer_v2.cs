using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

//メッシュを生成させる InputManagerで使用
public class LineDrawer_v2 : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer meshObject;

    [SerializeField]
    private int colliderAmount;

    [SerializeField]
    private Material material;

    [SerializeField]
    private float length = 0.1f;

    [SerializeField]
    private float width = 0.1f;

    //タッチ移動を格納
    List<Vector3> points = new List<Vector3>();

    //ポリゴンの頂点座標を格納
    List<Vector3> vertices = new List<Vector3>();
    List<Vector2> uvs = new List<Vector2>();

    //ポリゴンを描写するためのインデックスを格納
    List<int> triangles = new List<int>();

    int offset = 0;

    private int colliderIndex = 0;

    public Mesh mesh { get; private set; }

    private Mesh colliderMesh;

    private MeshFilter meshFilter;

    private MeshRenderer meshRenderer;

    private List<MeshCollider> meshColliders = new List<MeshCollider>();

    private Rigidbody _rigidbody;

    void Awake()
    {
        meshFilter = meshObject.gameObject.GetComponent<MeshFilter>();

        meshRenderer = meshObject.gameObject.GetComponent<MeshRenderer>();

        meshColliders.Add(meshObject.gameObject.GetComponent<MeshCollider>());

        for (int i = 0; i < colliderAmount; i++)
        {
            var addedCollider = meshObject.gameObject.AddComponent<MeshCollider>();
            addedCollider.convex = true;
            addedCollider.enabled = false;
            meshColliders.Add(addedCollider);
        }

        _rigidbody = meshObject.GetComponent<Rigidbody>();
    }

    void Start()
    {
        GetComponent<InputController>().finishDraw.Subscribe(i =>
        {
            /*
            meshFilter.sharedMesh = CreateWholeMesh();
            meshRenderer.material = material;

            meshRenderer.enabled = true;
            */

            colliderIndex = 0;

            //_rigidbody.isKinematic = false;
        });
    }

    Mesh CreateWholeMesh()
    {
        Mesh _mesh = new Mesh();
        _mesh.vertices = this.vertices.ToArray();

        _mesh.triangles = this.triangles.ToArray();
        // _mesh.uv = GetUV();
        _mesh.Optimize();

        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();
        _mesh.RecalculateTangents();

        return _mesh;
    }

    Vector2[] GetUV()
    {
        for (int i = 0; i < Mathf.CeilToInt((float)vertices.Count / (float)8); i++)
        {
            int k = i * 8;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));

            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(1, (float)k / (float)vertices.Count));
            k++;
            uvs.Add(new Vector2(0, (float)k / (float)vertices.Count));
        }
        return uvs.ToArray();
    }

    //メッシュを作成するメソッド
    void CreateMesh()
    {
        //1フレーム前のタッチ座標
        Vector3 prev = this.points[this.points.Count - 2];

        //最新のタッチ座標
        Vector3 top = this.points[this.points.Count - 1];

        Vector3 tmp = top - prev;
        //ドローが曲線を描く場合、prevとtopは斜めになる場合がある。
        //そういった際、足すxやyの値はその傾き具合によって変える必要があるため、ここで計算している
        Vector2 normal = new Vector2(tmp.y, -tmp.x).normalized * length;

        if (normal.x == 0 && normal.y == 0) return;//稀に値が0になる場合(prevとtopが近すぎる)があるためここで弾く

        //正面の面の座標を計算して使いまわす
        Vector2 p0 = (Vector2)prev + normal;//左下
        Vector2 t0 = (Vector2)top + normal;//右下
        Vector2 t1 = (Vector2)top - normal;//右上
        Vector2 p1 = (Vector2)prev - normal;//左上

        //曲線をより滑らかに見せる方法
        //1.メッシュ作成頻度を増やす
        //2.ドローされた座標よりも若干ずらして前のメッシュと重なるようにする
        //3.それぞれのメッシュの頂点が繋がるように頂点を計算する(難)
        //ポリゴンの頂点座標を取得
        //影の方線をうまく取得するには24この頂点が必要
        Vector3[] _vertices = GetVertices(prev, top, normal, p0, p1, t0, t1);

        // ポリゴンの頂点座標をリストに追加
        for (int i = 0; i < _vertices.Length; i++)
        {
            this.vertices.Add(_vertices[i]);
        }

        //各ポリゴンの頂点座標にインデックス番号を割り当て
        int[] _triangles =
        {
                offset, offset + 2, offset + 1,
                offset, offset + 3, offset + 2,
                offset + 5, offset + 4, offset + 7,
                offset + 5, offset + 7, offset + 6,
                offset + 8, offset + 10, offset + 9,
                offset + 8, offset + 11, offset + 10,
                offset + 12, offset + 14, offset + 13,
                offset + 12, offset + 15, offset + 14,
                offset + 16, offset + 18, offset + 17,
                offset + 16, offset + 19, offset + 18,
                offset + 20, offset + 22, offset + 21,
                offset + 20, offset + 23, offset + 22,
        };

        //割り当てたインデックス番号をリストに追加
        for (int i = 0; i < _triangles.Length; i++)
        {
            this.triangles.Add(_triangles[i]);
        }

        offset += 24;//インデックス番号の総計を更新

        //メッシュに割り当て
        mesh.vertices = this.vertices.ToArray();

        mesh.triangles = this.triangles.ToArray();

        mesh.Optimize();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        meshFilter.sharedMesh = mesh;

        meshRenderer.material = material;

        meshRenderer.enabled = true;

        SetColliderMesh(prev, top, p0, t0, t1, p1);

        //SetDebugSpheres();
    }

    private Vector3[] GetVertices(Vector3 prev, Vector3 top, Vector2 normal, Vector2 p0, Vector2 p1, Vector2 t0, Vector2 t1)
    {
        Vector3[] vertices;
        //1番初めのメッシュ生成の場合
        if (offset == 0)
        {
            Vector3[] _vertices =
            {
    /*0*/       new Vector3(p0.x, p0.y, prev.z),
    /*1*/       new Vector3(t0.x, t0.y, top.z),
    /*2*/       new Vector3(t1.x, t1.y, top.z),
    /*3*/       new Vector3(p1.x, p1.y, prev.z),
    /*4*/       new Vector3(p1.x, p1.y, prev.z + width),
    /*5*/       new Vector3(t1.x, t1.y, top.z + width),
    /*6*/       new Vector3(t0.x, t0.y, top.z + width),
    /*7*/       new Vector3(p0.x, p0.y, prev.z + width),
    /*8*/       new Vector3(p1.x, p1.y, prev.z + width),// face top
    /*9*/       new Vector3(p1.x, p1.y, prev.z),
    /*10*/      new Vector3(t1.x, t1.y, top.z),
    /*11*/      new Vector3(t1.x, t1.y, top.z + width),
    /*12*/      new Vector3(t0.x, t0.y, top.z + width), // face bottom
    /*13*/      new Vector3(t0.x, t0.y, top.z),
    /*14*/      new Vector3(p0.x, p0.y, prev.z),
    /*15*/      new Vector3(p0.x, p0.y, prev.z + width),
    /*16*/      new Vector3(t1.x, t1.y, top.z + width), // face right
    /*17*/      new Vector3(t1.x, t1.y, top.z),
    /*18*/      new Vector3(t0.x, t0.y, top.z),
    /*19*/      new Vector3(t0.x, t0.y, top.z + width),
    /*20*/      new Vector3(p0.x, p0.y, prev.z),// face left
    /*21*/      new Vector3(p1.x, p1.y, prev.z),
    /*22*/      new Vector3(p1.x, p1.y, prev.z + width),
    /*23*/      new Vector3(p0.x, p0.y, prev.z + width),
            };

            vertices = _vertices;
        }
        else//2つ目以降のメッシュ生成の場合
        {
            //1つ前のメッシュの右側の面の頂点座標を取得
            Vector3 pV1 = meshFilter.sharedMesh.vertices[offset - 23];
            Vector3 pV2 = meshFilter.sharedMesh.vertices[offset - 22];
            Vector3 pV4 = meshFilter.sharedMesh.vertices[offset - 20];
            Vector3 pV7 = meshFilter.sharedMesh.vertices[offset - 17];

            //取得した各頂点座標を現在のメッシュの左側の面に代入
            Vector3[] _vertices =
            { 
    /*0*/       pV2,
    /*1*/       new Vector3(t0.x, t0.y, top.z),
    /*2*/       new Vector3(t1.x, t1.y, top.z),
    /*3*/       pV1,
    /*4*/       pV4,
    /*5*/       new Vector3(t1.x, t1.y, top.z + width),
    /*6*/       new Vector3(t0.x, t0.y, top.z + width),
    /*7*/       pV7,
    /*8*/       pV4,// face top
    /*9*/       pV1,
    /*10*/      new Vector3(t1.x, t1.y, top.z),
    /*11*/      new Vector3(t1.x, t1.y, top.z + width),
    /*12*/      new Vector3(t0.x, t0.y, top.z + width), // face bottom
    /*13*/      new Vector3(t0.x, t0.y, top.z),
    /*14*/      pV2,
    /*15*/      pV7,
    /*16*/      new Vector3(t1.x, t1.y, top.z + width), // face right
    /*17*/      new Vector3(t1.x, t1.y, top.z),
    /*18*/      new Vector3(t0.x, t0.y, top.z),
    /*19*/      new Vector3(t0.x, t0.y, top.z + width),
    /*20*/      pV2,// face left
    /*21*/      pV1,
    /*22*/      pV4,
    /*23*/      pV7,
            };

            vertices = _vertices;
        }

        return vertices;
    }

    //コライダー用のメッシュを生成
    private void SetColliderMesh(Vector3 prev, Vector3 top, Vector2 fv1, Vector2 fv2, Vector2 fv3, Vector2 fv4)
    {
        //曲線をより滑らかに見せる方法
        //1.メッシュ作成頻度を増やす
        //2.ドローされた座標よりも若干ずらして前のメッシュと重なるようにする
        //3.それぞれのメッシュの頂点が繋がるように頂点を計算する(難)
        //ポリゴンの頂点座標を取得
        Vector3[] _vertices =
        {
            new Vector3(fv1.x , fv1.y, prev.z ),
            new Vector3(fv2.x , fv2.y , top.z ),
            new Vector3(fv3.x , fv3.y , top.z ),
            new Vector3(fv4.x , fv4.y ,prev.z ),
            new Vector3(fv4.x , fv4.y , prev.z + width),
            new Vector3(fv3.x , fv3.y , top.z + width),
            new Vector3(fv2.x , fv2.y , top.z + width),
            new Vector3(fv1.x , fv1.y , prev.z +  width),
        };

        colliderMesh.vertices = _vertices;

        int[] _triangles2 =
        {
            0, 2, 1,
            0, 3, 2,
            2, 3, 4,
            2, 4, 5,
            1, 2, 5,
            1, 5, 6,
            0, 7, 4,
            0, 4, 3,
            5, 4, 7,
            5, 7, 6,
            0, 6, 7,
            0, 1, 6,
        };

        colliderMesh.triangles = _triangles2;

        SetCollider();
    }

    //生成したコライダー用メッシュにmeshCollider適応するメソッド
    private void SetCollider()
    {
        if (colliderIndex == meshColliders.Count)
        {
            Debug.Log("no more collider");

            return;
        }

        meshColliders[colliderIndex].enabled = true;

        meshColliders[colliderIndex].sharedMesh = colliderMesh;

        colliderIndex++;
    }
    //メッシュの情報をリセットさせるメソッド
    public void PenDown(Vector3 tp)
    {
        meshRenderer.enabled = false;

        //メッシュの情報をリセット
        this.points.Clear();
        this.vertices.Clear();
        this.triangles.Clear();

        // 開始点を保存
        this.points.Add(tp);

        this.offset = 0;

        // メッシュ生成
        mesh = new Mesh();

        colliderMesh = new Mesh();

        meshObject.transform.position = Vector3.zero;

        meshObject.transform.eulerAngles = Vector3.zero;

        for (int i = 0; i < meshColliders.Count; i++)
        {
            meshColliders[i].enabled = false;
        }

        _rigidbody.isKinematic = true;
    }
    //マウスやタッチ入力がある間メッシュの更新をするメソッド
    public void PenMove(Vector3 tp)
    {
        //タッチしたポジションを追加
        this.points.Add(tp);
        //メッシュを生成
        CreateMesh();
    }

    public void PenUp()
    {
        _rigidbody.isKinematic = false;
    }
}
