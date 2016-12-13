using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop {
	public class ShopSampleButton : MonoBehaviour {

		public Button button;
		public Text nameLabel;
		public Text priceLabel;
		public Image iconImage;

		private ShopItem item;
		private ShopScrollList scrollList;

		public ShopItem Item { get { return item; } }

		// Use this for initialization
		void Start() {
			button.onClick.AddListener(() => { scrollList.TryTransferItemToOtherShop(this); });
		}
		public void Setup(ShopItem currentItem, ShopScrollList currentScrollList) {
			item = currentItem;
			nameLabel.text = item.itemName;
			priceLabel.text = item.price.ToString();
			iconImage.sprite = item.icon;
			scrollList = currentScrollList;
		}

		public void SetScrollList(ShopScrollList shopScrollList) {
			scrollList = shopScrollList;
			if (scrollList) {
				transform.SetParent(scrollList.contentPanel);
				transform.localScale = Vector3.one;
			} else {
				transform.SetParent(null);
			}
		}

	}
}
