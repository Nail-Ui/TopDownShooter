using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]

    // Inspector'da bu altta olan liste gözükecek
    public class Pool
    {
        public string _tag;           // "Mermi", "Patlama", "Düşman" gibi isimler
        public GameObject _prefab;    // Hangi nesneyi çoğaltacağız
        public int size;              // Kaç tane obje hazır olsun?
    }

    [SerializeField] private Transform poolContainer;

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    // Inspector'dan dolduracağımız havuz listesi
    public List<Pool> pools; // Birden fazla havuz olabilir (mermi, patlama, coin vs.)


    // Havuzları hızlı arayabilmek için kullanacağımız sözlük
    // Anahtar: tag (string) → Değer: hazır nesnelerin kuyruğu
    private Dictionary<string, Queue<GameObject>> _poolDictionary;

    private void Start()
    {
        // Sözlüğü oluşturuyoruz (boş bir sözlük)
        _poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Inspector'da tanımladığımız her havuz için bir döngü
        foreach (Pool pool in pools)
        {
            // Bu tag için yeni bir kuyruk (sıra) oluşturuyoruz
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                // Prefab'dan yeni bir kopya yaratıyoruz
                GameObject obj = Instantiate(pool._prefab, poolContainer); // Buraya parent verdik

                obj.name = pool._prefab.name + "(" + i + ")";
                // Ama hemen görünür olmasın diyoruz
                obj.SetActive(false);

                // Bu nesneyi kuyruğun sonuna ekliyoruz //Pool’a HER ZAMAN Instantiate edilen objeyi koyarsın, prefab’ı ASLA.
                objectPool.Enqueue(obj);
            }

            // For döngüsünde oluşan kuyruğu, tag ile eşleştirip sözlüğe kaydediyoruz
            _poolDictionary.Add(pool._tag, objectPool);

            // Kontrol amaçlı konsol çıktısı alıyoruz
            Debug.Log($"Pool created for tag: {pool._tag} with size {pool.size}");
        }
    }

    // Bu fonksiyon havuzdan nesne almak ve onu ".SetActive(true)" hale getirmek için kullanılır
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        // Böyle bir tag var mı diye kontrol ediyoruz
        if (!_poolDictionary.ContainsKey(tag))
        {
            // Eğer yoksa uyarı veriyoruz ve null dönüyoruz (hiçbir şey spawn olmayacak)
            Debug.LogWarning($"Pool with {tag} doesn't exist");
            return null;
        }

        Debug.Log("Spawn '" + tag + "' öncesi Queue count: " + _poolDictionary[tag].Count);

        // İlgili tag'in kuyruğundan en baştaki nesneyi alıyoruz (FIFO - ilk giren ilk çıkar) 
        // First-In, First-Out, is a fundamental accounting and inventory management method.
        GameObject objectsToSpawn = _poolDictionary[tag].Dequeue();

        // Aldığımız nesneyi görünür ve aktif hale getiriyoruz
        objectsToSpawn.SetActive(true);

        // Nesnenin pozisyonunu istediğimiz yere taşıyoruz
        objectsToSpawn.transform.position = position;

        // Nesnenin dönüşünü (rotation) istediğimiz değere ayarlıyoruz
        objectsToSpawn.transform.rotation = rotation;

        // Kullandıktan sonra nesneyi tekrar kuyruğun SONUNA geri koyuyoruz
        // Yani havuzda dolaşmaya devam edebilsin

        //_poolDictionary[tag].Enqueue(objectsToSpawn); Bu satırı ReturnToPool a taşıdık, yoksa mermiler art arda ateşlendiği 
        // zaman Queue'ya hemen geri dönüyordu

        // Aktive edilmiş, pozisyonu ve rotasyonu ayarlanmış nesneyi geri döndürüyoruz
        return objectsToSpawn;
    }


    // Ek coroutine

    public void ReturnToPool(string tag, GameObject obj)
    {
        obj.SetActive(false);
        //obj.transform.SetParent(poolContainer, true); //geri container'a koyuyoruz
        //obj.transform.localPosition = Vector3.zero;
        //obj.GetComponent<BulletController>().enabled = false;

        if (_poolDictionary.ContainsKey(tag))
        {
            // Kullandıktan sonra nesneyi tekrar kuyruğun SONUNA geri koyuyoruz
            // Yani havuzda dolaşmaya devam edebilsin
            _poolDictionary[tag].Enqueue(obj);
        }
    }
}
