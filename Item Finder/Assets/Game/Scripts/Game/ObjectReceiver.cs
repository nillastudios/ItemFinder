using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace NillaStudios
{
    public class ObjectReceiver : MonoBehaviour
    {
        public TextMeshPro classLabel;
        public Image coinMultiplierFill;

        [SerializeField]private ObjectClass targetObjectClass;
        private GameController gameController;

        private float multiplierDisappearTime;
        private bool startDisappear;

        public void Initialize(ObjectClass objectClass, float multiplierDisappearTime, GameController gameController)
        {
            this.targetObjectClass = objectClass;
            classLabel.text = targetObjectClass.ToString();
            this.multiplierDisappearTime = multiplierDisappearTime;

            this.gameController = gameController;
        }

        private void Update()
        {
            if(coinMultiplierFill.fillAmount >= 1f)
            {
                startDisappear = true;
            }
            else if(coinMultiplierFill.fillAmount <= 0f)
            {
                startDisappear = false;
            }

            if(startDisappear)
            {
                coinMultiplierFill.fillAmount -= Time.deltaTime / multiplierDisappearTime;
            }
        }

        public void FillCoinMeter()
        {
            StartCoroutine(FillCoinMeterRoutine());
        }

        private IEnumerator FillCoinMeterRoutine()
        {
            float timer = 0f;//(coinMultiplierFill.fillAmount);
            while(timer < 1f)
            {
                timer += Time.deltaTime;
                coinMultiplierFill.fillAmount = Mathf.Lerp(coinMultiplierFill.fillAmount, 1f, timer);
                yield return null;
            }

            yield break;
        } 

        private void OnTriggerEnter(Collider other)
        {
            PickableObject obj = other.GetComponent<PickableObject>();
            if(obj != null)
            {    
                gameController.ObjectReceived(obj, Mathf.Max(2, Mathf.RoundToInt(coinMultiplierFill.fillAmount * 5)));
            }
        }
    }
}
