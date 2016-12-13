using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Shop {
	[System.Serializable]
	public class ShopItem {
		public string itemName;
		public Sprite icon;
		public float price = 1f;
	}
	public class ShopScrollList : MonoBehaviour {
		public List<ShopItem> itemList;
		public ShopScrollList otherShop;
		public ShopSampleButton buttonPrefab;
		public Transform contentPanel;
		public Text myGoldDisplay;
		public float gold = 20f;

		private List<ShopSampleButton> buttonList;

		// Use this for initialization
		void Start() {
			buttonList = new List<ShopSampleButton>();
			AddButtons();
			RefreshDisplay();
		}

		public void RefreshDisplay() {
			myGoldDisplay.text = "Gold: " + gold.ToString();
		}

		private void AddButtons() {
			for (int i = 0; i < itemList.Count; i++) {
				ShopItem item = itemList[i];
				GameObject button = GameObject.Instantiate(buttonPrefab.gameObject);
				button.transform.SetParent(contentPanel);
				button.transform.localScale = Vector3.one;
				ShopSampleButton sampleButton = button.GetComponent<ShopSampleButton>();
				sampleButton.Setup(item, this);
			}
		}

		public void TryTransferItemToOtherShop(ShopSampleButton button) {
			ShopItem item = button.Item;
			if (otherShop.gold >= item.price) {
				gold += item.price;
				otherShop.gold -= item.price;
				this.RemoveItem(button);
				otherShop.AddItem(button);
				RefreshDisplay();
				otherShop.RefreshDisplay();
			}
		}
		private void AddItem(ShopSampleButton buttonToAdd) {
			itemList.Add(buttonToAdd.Item);
			buttonList.Add(buttonToAdd);
			buttonToAdd.SetScrollList(this);
		}
		private void RemoveItem(ShopSampleButton buttonToRemove) {
			itemList.Remove(buttonToRemove.Item);
			buttonList.Remove(buttonToRemove);
			buttonToRemove.SetScrollList(null);
		}
	}

}