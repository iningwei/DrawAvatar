using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZGame.EventExt;
using ZGame.Ress;
using ZGame.Window;

namespace ZGame.UGUIExtention
{
    public class Dropdown : MonoBehaviour
    {
        public Color SelectedColor = Color.white;
        public bool SelectedBold = false;
        private Color defaultColor = Color.white;
        private TMPro.FontStyles defaultFontStyle;
        public string[] initDatas;

        public UnityExtEvent onSelectionChanged = new UnityExtEvent();
        int curSelectionIndex = -1;

        Button dropdownBtn;
        RectTransform dropdownRect;
        GameObject dropItem;
        GameObject scrollView;
        RectTransform scrollViewRect;

        Transform contentTran;
        TMPro.TMP_Text shownTxt;

        GameObject[] items;
        public Canvas rootCanvas;
        void Awake()
        {
            this.dropItem = this.transform.Find("DropItem").gameObject;
            this.dropdownRect = this.transform.GetComponent<RectTransform>();

            this.scrollView = this.transform.Find("Scroll View").gameObject;
            this.scrollView.SetActive(false);
            this.scrollViewRect = this.scrollView.GetComponent<RectTransform>();

            this.contentTran = this.transform.Find("Scroll View/Viewport/Content");
            this.shownTxt = this.transform.Find("Shown").GetComponent<TMPro.TMP_Text>();
            defaultFontStyle = shownTxt.fontStyle;
            defaultColor = this.dropItem.FindChild("ContentDes").GetComponent<TMPro.TMP_Text>().color;
            if (rootCanvas == null)
            {
                rootCanvas = WindowManager.Instance.GetRootCanvas();
            }
            this.initItems();
            this.refreshItems();

            dropdownBtn = this.transform.GetComponent<Button>();
            dropdownBtn.onClick.AddListener(this.onDropdownBtnClicked);
        }

        public void SetInitData(string[] datas)
        {
            this.initDatas = datas;
            this.initItems();
            this.refreshItems();
        }
        public void SetInitDataAndIndex(string[] datas,int defaultIndex)
        {
            this.initDatas = datas;
            this.initItems();
            curSelectionIndex = defaultIndex;
            this.refreshItems();
        }
        public int GetCurIndex()
        {
            return this.curSelectionIndex;
        }

        private void onDropdownBtnClicked()
        {
            if (items == null || items.Length == 0)
            {
                return;
            }

            if (this.scrollView.activeSelf)
            {
                this.scrollView.SetActive(false);
            }
            else
            {
                this.scrollView.SetActive(true);
            }
        }

        void initItems()
        {
            if (initDatas == null)
            {
                return;
            }
            items = null;
            var count = initDatas.Length;
            if (count > 0)
            {
                curSelectionIndex = 0;
                this.contentTran.DestroyAllChilds(true);

                items = new GameObject[count];
                for (int i = 0; i < count; i++)
                {
                    int index = i;
                    GameObject itemObj = GameObjectHelper.Instantiate(dropItem);
                    itemObj.transform.SetParent(contentTran);
                    itemObj.transform.ResetLocalPRS();
                    itemObj.transform.name = "DropdownItem" + i;
                    itemObj.SetActive(true);
                    items[i] = itemObj;

                    Button itemBtn = itemObj.GetComponent<Button>();
                    itemBtn.onClick.RemoveAllListeners();
                    itemBtn.onClick.AddListener(() =>
                    {
                        this.scrollView.SetActive(false);
                        if (index != curSelectionIndex)
                        {
                            Debug.LogError(index + " clicked");
                            curSelectionIndex = index;
                            onSelectionChanged.Invoke(curSelectionIndex);
                            this.shownTxt.text = initDatas[index];

                            this.refreshItems();
                        }
                    });
                }
            }
        }

        void refreshItems()
        {
            if (items != null && items.Length > 0)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    var itemObj = items[i];
                    var checkMark = itemObj.transform.Find("Checkmark");
                    var contentDes = itemObj.transform.Find("ContentDes").GetComponent<TMPro.TMP_Text>();

                    if (i == curSelectionIndex)
                    {
                        checkMark.gameObject.SetActive(true);
                        contentDes.color = SelectedColor;
                        if (SelectedBold == true) 
                        { 
                            contentDes.fontStyle = TMPro.FontStyles.Bold; 
                        }
                        this.shownTxt.text = initDatas[i];
                    }
                    else
                    {
                        checkMark.gameObject.SetActive(false);
                        contentDes.color = defaultColor;
                        contentDes.fontStyle = defaultFontStyle;
                    }

                    contentDes.text = initDatas[i];
                }
            }
        }

        private void Update()
        {
            //松开的地方不在ScrollView区域内，则关闭
            if (rootCanvas != null && Input.GetMouseButtonUp(0) && this.scrollView.activeSelf)
            {
                if (UGUIUtility.IsPointerInRect(rootCanvas, this.dropdownRect))
                {
                    return;
                }
                if (!UGUIUtility.IsPointerInRect(rootCanvas, this.scrollViewRect))
                {
                    this.scrollView.SetActive(false);
                }
            }
        }


        private void OnDestroy()
        {
            this.dropdownBtn.onClick.RemoveAllListeners();
        }

    }
}
