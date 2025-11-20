using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Aftertime.SecretSome.BilliardsPrototype
{
    public class CardSpawnManager : MonoBehaviour
    {
        [SerializeField] private Vector2 _spawnXRange = new Vector2(2f, 14f);
        [SerializeField] private Vector2 _spawnYRange = new Vector2(1f, 5f);
        [SerializeField] private Vector2 _rotationRange = new Vector2(-40f, 40f);
        [SerializeField] private int _minSpawnCount = 4;
        [SerializeField] private int _maxSpawnCount = 8;
        [SerializeField] private float _minSpacing = 1.25f;
        [SerializeField] private CardObstacle[] _cardPool = new CardObstacle[0];
        [SerializeField] private Collider2D[] _blockedZones = new Collider2D[0];

        private readonly List<CardObstacle> _availableCards = new List<CardObstacle>();
        private readonly List<Vector2> _spawnedPositions = new List<Vector2>();

        private void OnEnable()
        {
            ArrowProjectile.onArrowExpired += HandleArrowExpired;
            RespawnCards();
        }

        private void OnDisable()
        {
            ArrowProjectile.onArrowExpired -= HandleArrowExpired;
        }

        private void HandleArrowExpired(ArrowProjectile.ArrowLifecycleResult result)
        {
            if (result.Origin != ArrowProjectile.ArrowOrigin.Primary)
                return;

            RespawnCards();
        }

        public void AssignPool(CardObstacle[] cards)
        {
            if (cards == null)
            {
                _cardPool = Array.Empty<CardObstacle>();
            }
            else
            {
                List<CardObstacle> filtered = new List<CardObstacle>();
                foreach (CardObstacle card in cards)
                {
                    if (card != null)
                        filtered.Add(card);
                }

                _cardPool = filtered.ToArray();
            }

            RespawnCards();
        }

        public void AssignBlockedZones(Collider2D[] zones)
        {
            _blockedZones = zones ?? Array.Empty<Collider2D>();
            RespawnCards();
        }


        private void RespawnCards()
        {
            _availableCards.Clear();
            _spawnedPositions.Clear();

            if (_cardPool == null || _cardPool.Length == 0)
                return;

            foreach (CardObstacle card in _cardPool)
            {
                if (card == null)
                    continue;

                card.gameObject.SetActive(false);
                _availableCards.Add(card);
            }

            if (_availableCards.Count == 0)
                return;

            int spawnCount = Mathf.Clamp(Random.Range(_minSpawnCount, _maxSpawnCount + 1), 0, _availableCards.Count);
            const int maxAttemptsPerCard = 12;

            for (int i = 0; i < spawnCount; i++)
            {
                if (_availableCards.Count == 0)
                    break;

                int index = Random.Range(0, _availableCards.Count);
                CardObstacle card = _availableCards[index];
                _availableCards.RemoveAt(index);

                Vector2 spawnPosition = Vector2.zero;
                bool foundPosition = false;
                for (int attempt = 0; attempt < maxAttemptsPerCard; attempt++)
                {
                    Vector2 candidate = new Vector2(
                        Random.Range(_spawnXRange.x, _spawnXRange.y),
                        Random.Range(_spawnYRange.x, _spawnYRange.y));

                    if (IsBlockedPosition(candidate))
                        continue;

                    bool overlaps = false;
                    foreach (Vector2 existing in _spawnedPositions)
                    {
                        if (Vector2.Distance(existing, candidate) < _minSpacing)
                        {
                            overlaps = true;
                            break;
                        }
                    }

                    if (overlaps == false)
                    {
                        spawnPosition = candidate;
                        foundPosition = true;
                        break;
                    }
                }

                if (foundPosition == false)
                    continue;

                float rotation = Random.Range(_rotationRange.x, _rotationRange.y);

                card.transform.position = spawnPosition;
                card.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
                card.ResetCard();
                _spawnedPositions.Add(spawnPosition);
            }
        }
    

        private bool IsBlockedPosition(Vector2 position)
        {
            if (_blockedZones == null || _blockedZones.Length == 0)
                return false;

            foreach (Collider2D zone in _blockedZones)
            {
                if (zone != null && zone.OverlapPoint(position))
                    return true;
            }

            return false;
        }
}
}
