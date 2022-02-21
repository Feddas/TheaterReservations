using GoogleSheetsToUnity.ThirdPary;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyJSON;
using UnityEngine;
using UnityEngine.Networking;

namespace GoogleSheetsToUnity
{
    public class SpreadsheetManagerRaw : SpreadsheetManager
    {
        /// <summary>
        /// Reads information from a spreadsheet
        /// </summary>
        /// <param name="search"></param>
        /// <param name="callback"></param>
        /// <param name="containsMergedCells"> does the spreadsheet contain merged cells, will attempt to group these by titles</param>
        public static void ReadRaw(GSTU_Search search, System.Action<ValueRange> callback, bool containsMergedCells = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("https://sheets.googleapis.com/v4/spreadsheets");
            sb.Append("/" + search.sheetId);
            sb.Append("/values");
            sb.Append("/" + search.worksheetName + "!" + search.startCell + ":" + search.endCell);
            sb.Append("?access_token=" + Config.gdr.access_token);

            UnityWebRequest request = UnityWebRequest.Get(sb.ToString());

            if (Application.isPlaying)
            {
                new Task(ReadRaw(request, search, containsMergedCells, callback));
            }
#if UNITY_EDITOR
            else
            {
                EditorCoroutineRunner.StartCoroutine(ReadRaw(request, search, containsMergedCells, callback));
            }
#endif
        }

        /// <summary>
        /// Reads the spread sheet and callback with the results
        /// </summary>
        /// <param name="request"></param>
        /// <param name="search"></param>
        /// <param name="containsMergedCells"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        static IEnumerator ReadRaw(UnityWebRequest request, GSTU_Search search, bool containsMergedCells, System.Action<ValueRange> callback)
        {
            if (Application.isPlaying)
            {
                yield return new Task(SpreadsheetManager.CheckForRefreshToken());
            }
#if UNITY_EDITOR
            else
            {
                yield return EditorCoroutineRunner.StartCoroutine(SpreadsheetManager.CheckForRefreshToken());
            }
#endif

            using (request)
            {
                yield return request.SendWebRequest();

                if (string.IsNullOrEmpty(request.downloadHandler.text) || request.downloadHandler.text == "{}")
                {
                    Debug.LogWarning("Unable to Retreive data from google sheets");
                    yield break;
                }

                var asJson = JSON.Load(request.downloadHandler.text);
                ValueRange rawData = asJson.Make<ValueRange>();

                if (callback != null)
                {
                    callback(rawData);
                }
            }
        }
    }
}
