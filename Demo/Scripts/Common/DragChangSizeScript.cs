using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SuperScrollView
{

    public class DragChangSizeScript :MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler
    {
        bool mIsDraging = false;

        public Camera mCamera;

        public float mBorderSize = 10;

        public Texture2D mCursorTexture;

        public Vector2 mCursorHotSpot = new Vector2(16, 16);

        RectTransform mCachedRectTransform;

        public System.Action mOnDragEndAction;
        public RectTransform CachedRectTransform
        {
            get
            {
                if (mCachedRectTransform == null)
                {
                    mCachedRectTransform = gameObject.GetComponent<RectTransform>();
                }
                return mCachedRectTransform;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetCursor(null, mCursorHotSpot, CursorMode.Auto);
        }

        void SetCursor(Texture2D texture, Vector2 hotspot, CursorMode cursorMode)
        {
            if (Input.mousePresent)
            {
                Cursor.SetCursor(texture, hotspot, cursorMode);
            }
        }

        void LateUpdate()
        {
            if (mCursorTexture == null)
            {
                return;
            }
            
            if(mIsDraging)
            {
                SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
                return;
            }

            Vector2 point;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, Input.mousePosition, mCamera, out point))
            {
                SetCursor(null, mCursorHotSpot, CursorMode.Auto);
                return;
            }
            float d = CachedRectTransform.rect.width - point.x;
            if(d < 0)
            {
                SetCursor(null, mCursorHotSpot, CursorMode.Auto);
            }
            else if ( d <= mBorderSize)
            {
                SetCursor(mCursorTexture, mCursorHotSpot, CursorMode.Auto);
            }
            else
            {
                SetCursor(null, mCursorHotSpot, CursorMode.Auto);
            }

        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            mIsDraging = true;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            mIsDraging = false;
            if(mOnDragEndAction != null)
            {
                mOnDragEndAction();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 p1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(CachedRectTransform, eventData.position, mCamera, out p1);
            if(p1.x <= 0)
            {
                return;
            }
            CachedRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, p1.x);
        }

    }
}
