using System;
using UnityEngine;
using UnityEngine.UI;
namespace Kit {
    public class UIImageNum : MonoBehaviour {
        public Image[] imageNums;
        public string numPrefix;
        public bool setNativeSize;

        public void ShowNum(int num, Func<string, Sprite> loadSprite) {
            if (imageNums == null || imageNums.Length == 0) {
                return;
            }
            if (loadSprite == null) {
                return;
            }
            var maxNum = 1;
            foreach (var imageNum in imageNums) {
                imageNum.SetActiveEx(false);
                maxNum *= 10;
            }
            maxNum -= 1;
            if (num > maxNum) {
                num = maxNum;
            }
            if (num < 0) {
                num = 0;
            }

            var tmpNumber = num;
            var tmpDigit = 0;
            var tmpIndex = 0;

            if (tmpNumber != 0) {
                while (tmpNumber != 0) {
                    tmpDigit = tmpNumber % 10;
                    tmpNumber = (tmpNumber - tmpDigit) / 10;
                    var image = imageNums[tmpIndex];
                    image.sprite = loadSprite($"{numPrefix}{tmpDigit}");
                    if (setNativeSize) {
                        image.SetNativeSize();
                    }
                    image.SetActiveEx(true);
                    ++tmpIndex;
                }
            } else {
                var image = imageNums[tmpIndex];
                image.sprite = loadSprite($"{numPrefix}{tmpDigit}");
                if (setNativeSize) {
                    image.SetNativeSize();
                }
            }
        }
    }
}
