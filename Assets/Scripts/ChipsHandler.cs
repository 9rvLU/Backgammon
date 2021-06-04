using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ChipsHandler : MonoBehaviour
{
    // フィールド
    [SerializeField] int id = 0;
    [SerializeField] string chipColor = "black";


    int _activeChipNum;
    int _shownChipNum;


    Material _material;
    Transform _allChips;


    bool _isSelectable;

    bool _mouseOver;
    bool _mouseDraging;


    GameObject _floatingChip;


    void Start()
    {
        Debug.Log(id);


        /*
        //ExecuteEvents.Executeを使ってメソッド実行
        ExecuteEvents.Execute<IChipMover>(
          target: GameObject.Find("Scripts/BoardHandler"),
          eventData: null,
          functor: (reciever, eventData) => reciever.GetAfterPoint("black", 6, 3)
        ) ;
        */


        // 自分のマテリアルを取得
        _material = this.gameObject.GetComponent<Renderer>().material;

        // 子要素を取得
        _allChips = this.gameObject.GetComponentInChildren<Transform>(true);

        // 選択不可能かつ非選択状態
        _isSelectable = true;
        _mouseDraging = false;
        _mouseOver = false;


        _activeChipNum = 3;
        _shownChipNum = _activeChipNum;


        _floatingChip = GameObject.Find("Chips/FloatingChip");
    }

    void Update()
    {
        UpdatePointMat();
        UpdateChipsMat();
    }

    // マウスカーソルがオブジェクトに乗った時の処理
    private void OnMouseEnter()
    {
        if (_isSelectable)
        {
            _mouseOver = true;


            //this._activeChipNum = 3;
        }
    }

    // マウスカーソルがオブジェクトから離れた時の処理
    private void OnMouseExit()
    {
        _mouseOver = false;


        //this._activeChipNum = 2;
    }

    // マウスが押された時の処理
    private void OnMouseDown()
    {
        _mouseDraging = true;
        _shownChipNum = _activeChipNum - 1;
    }

    // マウスが押されてから離された時の処理
    private void OnMouseUp()
    {
        var floatingChip = GameObject.Find("FloatingChip").transform.position = new Vector3(0, 1000, 0);


        // カメラからマウスカーソルへのレイを取得
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // レイがタグ"Player"のついたオブジェクトにあたった時の処理
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.tag == "ChipCollection")
            {
                //_isGrabbing = true;
                //_point = hit.collider.gameObject;

                if (this.GetComponent<Collider>() == hit.collider)
                {
                    // 
                    Debug.Log($"this is me!");
                }
                else
                {
                    Debug.Log($"Who are you?");
                }
            }
        }


        Debug.Log($"Im {this}");


        _shownChipNum = _activeChipNum;
        _mouseDraging = false;
    }

    private void OnMouseDrag()
    {
        var plane = new Plane(Vector3.up, 100 * Vector3.up);


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float Intersection;
        plane.Raycast(ray, out Intersection);

        _floatingChip.transform.position = ray.GetPoint(Intersection);
    }

    private void UpdatePointMat()
    {
        Color translucentWhite = new Color(0f, 0f, 0f, 0.2f);
        Color transparent = Color.clear;

        Color emissionBlue = Color.blue;
        Color emissionOrange = new Color(1f, 0.5f, 0f);
        Color emissionGreen = new Color(
            0f,                                                         // red
            0.5f + 0.5f * Mathf.Sin(120 * Time.time * Mathf.Deg2Rad),   // green
            0f                                                          // blue
        );

        // 選択可能なとき、emissionをon
        if (_isSelectable)
        {
            _material.SetColor("_Color", translucentWhite);
            _material.EnableKeyword("_EMISSION");
        }
        else
        {
            _material.SetColor("_Color", transparent);
            _material.DisableKeyword("_EMISSION");
        }


        // マウスが乗っているとき、オレンジに発光
        if (_mouseOver)
        {
            _material.SetColor("_EmissionColor", emissionOrange);
        }
        else
        {
            _material.SetColor("_EmissionColor", emissionGreen);
        }


        // マウスドラッグ中、灰色に発光
        if (_mouseDraging)
        {
            _material.SetColor("_EmissionColor", emissionBlue);
        }
        else
        {
            _material.SetColor("_EmissionColor", emissionGreen);
        }
    }
    private void UpdateChipsMat()
    {
        Color white = Color.white;
        Color black = Color.black;

        Color emissionBlue = new Color(0f, 0f, 0.2f);
        Color emissionOrange = new Color(0.2f, 0.1f, 0f);
        Color emissionGreen = new Color(
            0f,                                                         // red
            0.1f + 0.1f * Mathf.Sin(120 * Time.time * Mathf.Deg2Rad),   // green
            0f                                                          // blue
        );



        // 表示するチップの数を変更
        int cnt = 0;
        foreach (Transform item in _allChips)
        {
            var mat = item.GetChild(0).GetComponent<MeshRenderer>().material;


            // 選択可能なとき、emissionをon
            if (_isSelectable)
            {
                mat.EnableKeyword("_EMISSION");
            }
            else
            {
                mat.DisableKeyword("_EMISSION");
            }


            // マウスが乗っているとき、オレンジに発光
            if (_mouseOver)
            {
                mat.SetColor("_EmissionColor", emissionOrange);
            }
            else
            {
                mat.SetColor("_EmissionColor", emissionGreen);
            }


            // マウスドラッグ中、青色に発光
            if (_mouseDraging)
            {
                _material.SetColor("_EmissionColor", emissionBlue);
            }
            else
            {
                _material.SetColor("_EmissionColor", emissionGreen);
            }


            // マテリアルの色を変更
            switch (chipColor)
            {
                case "white":
                    mat.SetColor("_Color", white);
                    break;
                case "black":
                    mat.SetColor("_Color", black);
                    break;
                default:
                    break;
            }


            // チップをアクティブ/非アクティブ化
            if (cnt < _shownChipNum)
            {
                item.gameObject.SetActive(true);
            }
            else
            {
                item.gameObject.SetActive(false);
            }


            cnt++;
        }
    }
}
