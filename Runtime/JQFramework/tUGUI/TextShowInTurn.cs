using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JQFramework.tUGUI
{

    public class TextShowInTurn : MonoBehaviour
    {
        // Start is called before the first frame update
        public bool isActive = false;
        private float timer = 0;
        public int currentPos = 1;
        public float charsPerSecond = 0.05f;//多久打一个字
        public string words;//保存需要显示的文字
        public Text myText;//获取身上的test脚本
        private char[] strCharArr; //把字符串切割成一个个字符存于此数组
        List<int> colorJumpList = new List<int>();//颜色字跳过的索引
        //List<int> colorJumpList = new List<int>();
        void Start()
        {
            if (words!="")
            {
                checkColorsInWords();
            }
        }


        public void setWord(string word)
        {
            this.words = word;
            timer = 0;
            currentPos = 0;
            myText.text = "";
            isActive = true;
            checkColorsInWords();
        }

        //把字符串分割存起来
        private void checkColorsInWords()
        {
            strCharArr = words.ToCharArray();
        
            //Debug.LogError("字符串数量" + words.Length+ "字符数组数量"+ strCharArr.Length);
            for (int i = 0; i < strCharArr.Length; i++)
            {

                char singleChar = strCharArr[i];
                if (singleChar=='>')
                {
                    colorJumpList.Add(i + 1);
                }
                //print("字符打印："+ singleChar);
         
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (isActive)
            {
                timer += Time.deltaTime;
                if (timer >= charsPerSecond)//判断计时器时间是否到达
                {
                    timer = 0;
              

                    if (currentPos != words.Length)
                    {
             
                        if (strCharArr.Length== words.Length && strCharArr[currentPos]=='<')
                        {
                            currentPos = getColorJumpIndex();

                        }
                        else
                        {
                            currentPos++;
                        }
                    }
                

                    myText.text = words.Substring(0, currentPos);//刷新文本显示内容

                    if (currentPos >= words.Length)
                    {
                        OnFinish();
                    }
                }

            }
        }
        int colorStringEndIndex = 0;

        //获取跳转索引
        int getColorJumpIndex()
        {
            //int[] indexList = colorJumpList.Keys.to;
            colorStringEndIndex = colorStringEndIndex + 2;
            int i = 1;
            int tempInex=0;
            if (true)
            {

            }
            foreach (int colorIndexData in colorJumpList)
            {
                //if (colorIndexData.Value == false)
                //{
                //    colorJumpList[colorIndexData.Key] = true;
                //}
                tempInex = colorIndexData;

                if (i>=colorStringEndIndex)
                {
                    return tempInex;
                }
                i = i + 1; 
            }
            return tempInex;

        }

        public void OnFinish()
        {
            isActive = false;
            timer = 0;
            currentPos = 0;
            myText.text = words;
            colorStringEndIndex = 0;
        }
    }
}
