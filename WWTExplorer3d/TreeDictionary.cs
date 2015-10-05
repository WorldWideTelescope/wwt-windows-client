//------------------------------------------------------------------------------
// Abstract implementation of a Red-Black tree
//
// <copyright file="TreeDictionary.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//
// File History:
//           paulkoch        June 19, 2007        created
//
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

// bugbug/todo list
//
// - Implement non-generic IComparable on all object that implement IComparer<...>.
// 
// - ensure that Enumerator state is consistent.  Make the higher or lower flags reflect the
//   intent of the TraversalStartingPoint enum, and also make it point to the first node to 
//   be returned if such a node exists, and set that node when the user reverses direction
//   if the node is fixed at one of the ends
// 
// - can hoist one of the index checks in StartByWeight so that it happens in the initial 
//   exception check instead (for the case of an empty list, weight == totalweight == 0
//
// - can optimize ...FromKey & FromWeight & FromDirection by making comparisons >= or <= instead of == and then the rest of the check
// 
using System.Diagnostics.CodeAnalysis;

namespace MicrosoftInternal.AdvancedCollections
{
    public enum TraversalDirection { LowToHigh, HighToLow };
    public enum TraversalStartingPoint { EqualOrError, EqualOrLess, EqualOrMore, Less, More };

    public sealed class TreeDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ICollection<KeyValuePair<TKey, TValue>>, IEnumerable<KeyValuePair<TKey, TValue>>, IDictionary, ICollection, IEnumerable
    {
        readonly IComparer<TKey> comparer;
        readonly bool isAllowedDuplicates;

        int count;
        readonly KeyCollection keys;
        internal TreeNode loopbackNode;
        readonly object syncRoot = new object();
        readonly ValueCollection values;

        public TreeDictionary()
        {
            comparer = Comparer<TKey>.Default;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);
        }

        public TreeDictionary(bool isAllowedDuplicates)
        {
            comparer = Comparer<TKey>.Default;
            this.isAllowedDuplicates = isAllowedDuplicates;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);
        }

        public TreeDictionary(IComparer<TKey> comparer)
        {
            this.comparer = comparer;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);
        }

        public TreeDictionary(IDictionary<TKey, TValue> dictionary)
        {
            comparer = Comparer<TKey>.Default;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);

            AddDictionary(dictionary);
        }

        public TreeDictionary(IComparer<TKey> comparer, bool isAllowedDuplicates)
        {
            this.comparer = comparer;
            this.isAllowedDuplicates = isAllowedDuplicates;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);
        }

        public TreeDictionary(IDictionary<TKey, TValue> dictionary, bool isAllowedDuplicates)
        {
            comparer = Comparer<TKey>.Default;
            this.isAllowedDuplicates = isAllowedDuplicates;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);

            AddDictionary(dictionary);
        }

        public TreeDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer)
        {
            this.comparer = comparer;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);

            AddDictionary(dictionary);
        }

        public TreeDictionary(IDictionary<TKey, TValue> dictionary, IComparer<TKey> comparer, bool isAllowedDuplicates)
        {
            this.comparer = comparer;
            this.isAllowedDuplicates = isAllowedDuplicates;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;

            keys = new KeyCollection(this);
            values = new ValueCollection(this);

            AddDictionary(dictionary);
        }

        public TValue this[TKey key]
        {
            get
            {
                var currentNode = loopbackNode.Parent;
                TreeNode nextNode;
                int comparison;

                while(currentNode != loopbackNode)
                {
                    nextNode = currentNode.Right;
                    comparison = comparer.Compare(key, currentNode.Key);
                    if(comparison <= 0)
                    {
                        if(comparison == 0)
                        {
                            return currentNode.Value;
                        }
                        nextNode = currentNode.Left;
                    }
                    currentNode = nextNode;
                }

                throw new InvalidOperationException("TreeDictionary does not contain this key");
            }
            set
            {
                Add(key, value, 0, true);
            }
        }

        public IComparer<TKey> Comparer
        {
            get
            {
                return comparer;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsAllowedDuplicates
        {
            get
            {
                return isAllowedDuplicates;
            }
        }

        public KeyCollection Keys
        {
            get
            {
                return keys;
            }
        }

        public int TotalWeight
        {
            get
            {
                return loopbackNode.Parent.Weight;
            }
        }

        public ValueCollection Values
        {
            get
            {
                return values;
            }
        }

        public int Add(TKey key, TValue value)
        {
            return Add(key, value, 0, false);
        }

        public int Add(TKey key, TValue value, int weight)
        {
            return Add(key, value, weight, false);
        }

        public void Clear()
        {
            count = 0;

            // make all existing enumerators on the old tree invalid.  The loopbackNode is never Red, so let's set that here
            loopbackNode.IsRed = true;

            loopbackNode = new TreeNode();
            loopbackNode.Parent = loopbackNode;
            loopbackNode.Left = loopbackNode;
            loopbackNode.Right = loopbackNode;
        }

        public bool ContainsKey(TKey key)
        {
            var currentNode = loopbackNode.Parent;
            TreeNode nextNode;
            int comparison;

            while(currentNode != loopbackNode)
            {
                nextNode = currentNode.Right;
                comparison = comparer.Compare(key, currentNode.Key);
                if(comparison <= 0)
                {
                    if(comparison == 0)
                    {
                        return true;
                    }
                    nextNode = currentNode.Left;
                }
                currentNode = nextNode;
            }

            return false;
        }

        [SuppressMessage("Microsoft.Usage", "CA2233", Justification = "Potential overflow has been addressed in the header instead of checking at each assignment")]
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if(array.Length < arrayIndex + Count)
            {
                throw new ArgumentException("array is not large enough to store this collection", "array");
            }

            var localLoopbackNode = loopbackNode;
            var currentNode = localLoopbackNode.Left;
            TreeNode nextNode;

            if(currentNode == localLoopbackNode)
            {
                return;
            }

            while(true)
            {
                array[arrayIndex] = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                ++arrayIndex;
                nextNode = currentNode.Right;
                if(nextNode == localLoopbackNode)
                {
                    if(currentNode != localLoopbackNode.Right)
                    {
                        while(true)
                        {
                            nextNode = currentNode.Parent;
                            if(nextNode.Left != currentNode)
                            {
                                currentNode = nextNode;
                                continue;
                            }
                            currentNode = nextNode;
                            break;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    currentNode = nextNode;
                    while(true)
                    {
                        nextNode = currentNode.Left;
                        if(nextNode != localLoopbackNode)
                        {
                            currentNode = nextNode;
                            continue;
                        }
                        break;
                    }
                }
            }
        }

        public int GetCumulativeWeight(TKey key)
        {
            var localLoopbackNode = loopbackNode;
            var currentNode = localLoopbackNode.Parent;
            TreeNode nextNode;
            var cumulativeWeight = 0;
            while(currentNode != localLoopbackNode)
            {
                nextNode = currentNode.Left;
                var comparison = comparer.Compare(key, currentNode.Key);
                if(comparison >= 0)
                {
                    if(comparison == 0)
                    {
                        return cumulativeWeight + nextNode.Weight;
                    }
                    nextNode = currentNode.Right;
                    cumulativeWeight += currentNode.Weight - nextNode.Weight;
                }
                currentNode = nextNode;
            }
            throw new ArgumentException("key does not exist in the tree", "key");
        }

        public int GetCumulativeWeight(TreeDictionaryKeyEnumerator<TKey, TValue> enumerator)
        {
            var currentNode = enumerator.node;
            var localLoopbackNode = enumerator.loopbackNode;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(currentNode.Parent == null || localLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(loopbackNode != localLoopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryKeyEnumerator is not positioned on a valid item");
            }

            var cumulativeWeight = currentNode.Left.Weight;

            while(true)
            {
                var nextNode = currentNode.Parent;

                if(nextNode == localLoopbackNode)
                {
                    return cumulativeWeight;
                }
                if(nextNode.Right == currentNode)
                {
                    cumulativeWeight += nextNode.Weight - currentNode.Weight;
                }

                currentNode = nextNode;
            }
        }

        public int GetCumulativeWeight(TreeDictionaryKeyValuePairEnumerator<TKey, TValue> enumerator)
        {
            var currentNode = enumerator.node;
            var localLoopbackNode = enumerator.loopbackNode;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(currentNode.Parent == null || localLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(loopbackNode != localLoopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryKeyValuePairEnumerator is not positioned on a valid item");
            }

            var cumulativeWeight = currentNode.Left.Weight;

            while(true)
            {
                var nextNode = currentNode.Parent;

                if(nextNode == localLoopbackNode)
                {
                    return cumulativeWeight;
                }
                if(nextNode.Right == currentNode)
                {
                    cumulativeWeight += nextNode.Weight - currentNode.Weight;
                }

                currentNode = nextNode;
            }
        }

        public int GetCumulativeWeight(TreeDictionaryValueEnumerator<TKey, TValue> enumerator)
        {
            var currentNode = enumerator.node;
            var localLoopbackNode = enumerator.loopbackNode;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(currentNode.Parent == null || localLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(loopbackNode != localLoopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryValueEnumerator is not positioned on a valid item");
            }

            var cumulativeWeight = currentNode.Left.Weight;

            while(true)
            {
                var nextNode = currentNode.Parent;

                if(nextNode == localLoopbackNode)
                {
                    return cumulativeWeight;
                }
                if(nextNode.Right == currentNode)
                {
                    cumulativeWeight += nextNode.Weight - currentNode.Weight;
                }

                currentNode = nextNode;
            }
        }

        public TreeDictionaryKeyValuePairEnumerator<TKey, TValue> GetEnumerator()
        {
            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(this, loopbackNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit);
        }

        public KeyValuePair<TKey, TValue> GetFromKey(TKey key, TraversalStartingPoint startingPoint)
        {
            KeyValuePair<TKey, TValue> keyValuePair;
            if(TryGetFromKey(key, startingPoint, out keyValuePair))
            {
                return keyValuePair;
            }
            throw new InvalidOperationException("This TreeDictionary does not contain this key");
        }

        public KeyValuePair<TKey, TValue> GetFromWeight(int weight)
        {
            KeyValuePair<TKey, TValue> keyValuePair;
            if(TryGetFromWeight(weight, out keyValuePair))
            {
                return keyValuePair;
            }
            throw new InvalidOperationException("This TreeDictionary does not contain this key");
        }

        public int GetItemWeight(TKey key)
        {
            var localLoopbackNode = loopbackNode;
            var currentNode = localLoopbackNode.Parent;
            TreeNode nextNode;
            while(currentNode != localLoopbackNode)
            {
                nextNode = currentNode.Left;
                var comparison = comparer.Compare(key, currentNode.Key);
                if(comparison >= 0)
                {
                    if(comparison == 0)
                    {
                        return currentNode.Weight - currentNode.Left.Weight - currentNode.Right.Weight;
                    }
                    nextNode = currentNode.Right;
                }
                currentNode = nextNode;
            }

            throw new ArgumentException("key does not exist in the tree", "key");
        }

        public int GetItemWeight(TreeDictionaryKeyEnumerator<TKey, TValue> enumerator)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryKeyEnumerator is not positioned on a valid item");
            }

            return enumeratorNode.Weight - enumeratorNode.Left.Weight - enumeratorNode.Right.Weight;
        }

        public int GetItemWeight(TreeDictionaryKeyValuePairEnumerator<TKey, TValue> enumerator)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryKeyValuePairEnumerator is not positioned on a valid item");
            }

            return enumeratorNode.Weight - enumeratorNode.Left.Weight - enumeratorNode.Right.Weight;
        }

        public int GetItemWeight(TreeDictionaryValueEnumerator<TKey, TValue> enumerator)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot get this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("TreeDictionaryValueEnumerator is not positioned on a valid item");
            }

            return enumeratorNode.Weight - enumeratorNode.Left.Weight - enumeratorNode.Right.Weight;
        }

        public void SetItemWeight(TKey key, int newWeight)
        {
            var localLoopbackNode = loopbackNode;
            var currentNode = localLoopbackNode.Parent;
            TreeNode nextNode;
            while(true)
            {
                if(currentNode == localLoopbackNode)
                    throw new ArgumentException("key does not exist in the tree", "key");

                nextNode = currentNode.Left;
                var comparison = comparer.Compare(key, currentNode.Key);
                if(comparison >= 0)
                {
                    if(comparison == 0)
                    {
                        break;
                    }
                    nextNode = currentNode.Right;
                }
                currentNode = nextNode;
            }

            newWeight = newWeight - currentNode.Weight + currentNode.Left.Weight + currentNode.Right.Weight;

            if(newWeight != 0)
            {
                do
                {
                    currentNode.Weight += newWeight;
                    currentNode = currentNode.Parent;
                } while(currentNode != localLoopbackNode);
            }
        }

        public void SetItemWeight(TreeDictionaryKeyEnumerator<TKey, TValue> enumerator, int newWeight)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot set this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }

            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }

            Debug.Assert(enumeratorNode != enumeratorLoopbackNode);
            newWeight = newWeight - enumeratorNode.Weight + enumeratorNode.Left.Weight + enumeratorNode.Right.Weight;

            if(newWeight != 0)
            {
                do
                {
                    enumeratorNode.Weight += newWeight;
                    enumeratorNode = enumeratorNode.Parent;
                } while(enumeratorNode != enumeratorLoopbackNode);
            }
        }

        public void SetItemWeight(TreeDictionaryKeyValuePairEnumerator<TKey, TValue> enumerator, int newWeight)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot set this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }
            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }

            Debug.Assert(enumeratorNode != enumeratorLoopbackNode);
            newWeight = newWeight - enumeratorNode.Weight + enumeratorNode.Left.Weight + enumeratorNode.Right.Weight;

            if(newWeight != 0)
            {
                do
                {
                    enumeratorNode.Weight += newWeight;
                    enumeratorNode = enumeratorNode.Parent;
                } while(enumeratorNode != enumeratorLoopbackNode);
            }
        }

        public void SetItemWeight(TreeDictionaryValueEnumerator<TKey, TValue> enumerator, int newWeight)
        {
            var enumeratorLoopbackNode = enumerator.loopbackNode;
            var enumeratorNode = enumerator.node;

            // if this node has been set to null, then it has been deleted
            // if the loopbackNode has been set to red (which is an invalid state in the red-black tree, then the entire tree
            // has been deleted via the Clear() function
            if(enumeratorNode.Parent == null || enumeratorLoopbackNode.IsRed)
            {
                throw new InvalidOperationException("The node which the enumerator was located on was deleted");
            }

            if(enumeratorLoopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot set this item's weight because the enumerator was not created by this TreeDictionary", "enumerator");
            }
            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }

            Debug.Assert(enumeratorNode != enumeratorLoopbackNode);
            newWeight = newWeight - enumeratorNode.Weight + enumeratorNode.Left.Weight + enumeratorNode.Right.Weight;

            if(newWeight != 0)
            {
                do
                {
                    enumeratorNode.Weight += newWeight;
                    enumeratorNode = enumeratorNode.Parent;
                } while(enumeratorNode != enumeratorLoopbackNode);
            }
        }

        public StartFromDirectionCollection StartFromDirection(TraversalDirection direction)
        {
            return new StartFromDirectionCollection(this, direction);
        }

        public StartFromKeyCollection StartFromKey(TKey key)
        {
            return new StartFromKeyCollection(this, key, TraversalStartingPoint.EqualOrError, TraversalDirection.LowToHigh);
        }

        public StartFromKeyCollection StartFromKey(TKey key, TraversalDirection direction)
        {
            return new StartFromKeyCollection(this, key, TraversalStartingPoint.EqualOrError, direction);
        }

        public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint)
        {
            return new StartFromKeyCollection(this, key, startingPoint, TraversalDirection.LowToHigh);
        }

        public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
        {
            return new StartFromKeyCollection(this, key, startingPoint, direction);
        }

        public StartFromWeightCollection StartFromWeight(int weight)
        {
            return new StartFromWeightCollection(this, weight, TraversalDirection.LowToHigh);
        }

        public StartFromWeightCollection StartFromWeight(int weight, TraversalDirection direction)
        {
            return new StartFromWeightCollection(this, weight, direction);
        }

        public bool Remove(TKey key)
        {
            var currentNode = loopbackNode.Parent;
            TreeNode nextNode;
            int comparison;

            while(currentNode != loopbackNode)
            {
                nextNode = currentNode.Right;
                comparison = comparer.Compare(key, currentNode.Key);

                if(comparison <= 0)
                {
                    if(comparison == 0)
                    {
                        Remove(currentNode);
                        return true;
                    }
                    nextNode = currentNode.Left;
                }
                currentNode = nextNode;
            }

            return false;
        }

        public bool RemoveThenMoveNext(TreeDictionaryKeyEnumerator<TKey, TValue> enumerator)
        {
            if(enumerator.loopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot delete item because the enumerator was not created by this TreeDictionary.", "enumerator");
            }
            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }
            var deleteNode = enumerator.node;
            var result = enumerator.MoveNext();
            Remove(deleteNode);
            return result;
        }

        public bool RemoveThenMoveNext(TreeDictionaryKeyValuePairEnumerator<TKey, TValue> enumerator)
        {
            if(enumerator.loopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot delete item because the enumerator was not created by this TreeDictionary.", "enumerator");
            }
            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }
            var deleteNode = enumerator.node;
            var result = enumerator.MoveNext();
            Remove(deleteNode);
            return result;
        }

        public bool RemoveThenMoveNext(TreeDictionaryValueEnumerator<TKey, TValue> enumerator)
        {
            if(enumerator.loopbackNode != loopbackNode)
            {
                throw new ArgumentException("Cannot delete item because the enumerator was not created by this TreeDictionary.", "enumerator");
            }
            if(enumerator.statusBits != 0)
            {
                throw new InvalidOperationException("enumerator not positioned on a valid item");
            }
            var deleteNode = enumerator.node;
            var result = enumerator.MoveNext();
            Remove(deleteNode);
            return result;
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
        public bool TryGetFromKey(TKey key, TraversalStartingPoint startingPoint, out KeyValuePair<TKey, TValue> keyValuePair)
        {
            TreeNode previousValidNode = null;
            var currentNode = loopbackNode.Parent;
            int comparison;

            switch(startingPoint)
            {
                case TraversalStartingPoint.EqualOrError:
                    while(currentNode != loopbackNode)
                    {
                        comparison = comparer.Compare(key, currentNode.Key);

                        if(comparison == 0)
                        {
                            keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                            return true;
                        }

                        currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                    }

                    keyValuePair = default(KeyValuePair<TKey, TValue>);
                    return false;

                case TraversalStartingPoint.EqualOrLess:
                    while(currentNode != loopbackNode)
                    {
                        comparison = comparer.Compare(key, currentNode.Key);

                        if(0 <= comparison)
                        {
                            if(comparison == 0)
                            {
                                keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                                return true;
                            }
                            previousValidNode = currentNode;
                        }

                        currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                    }

                    if(previousValidNode == null)
                    {
                        keyValuePair = default(KeyValuePair<TKey, TValue>);
                        return false;
                    }
                    keyValuePair = new KeyValuePair<TKey, TValue>(previousValidNode.Key, previousValidNode.Value);
                    return true;
                case TraversalStartingPoint.EqualOrMore:
                    while(currentNode != loopbackNode)
                    {
                        comparison = comparer.Compare(key, currentNode.Key);

                        if(comparison <= 0)
                        {
                            if(comparison == 0)
                            {
                                keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                                return true;
                            }
                            previousValidNode = currentNode;
                        }

                        currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                    }

                    if(previousValidNode == null)
                    {
                        keyValuePair = default(KeyValuePair<TKey, TValue>);
                        return false;
                    }
                    keyValuePair = new KeyValuePair<TKey, TValue>(previousValidNode.Key, previousValidNode.Value);
                    return true;

                case TraversalStartingPoint.Less:
                    while(currentNode != loopbackNode)
                    {
                        comparison = comparer.Compare(key, currentNode.Key);

                        if(0 < comparison)
                        {
                            previousValidNode = currentNode;
                        }

                        currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                    }

                    if(previousValidNode == null)
                    {
                        keyValuePair = default(KeyValuePair<TKey, TValue>);
                        return false;
                    }
                    keyValuePair = new KeyValuePair<TKey, TValue>(previousValidNode.Key, previousValidNode.Value);
                    return true;

                case TraversalStartingPoint.More:
                    while(currentNode != loopbackNode)
                    {
                        comparison = comparer.Compare(key, currentNode.Key);

                        if(comparison < 0)
                        {
                            previousValidNode = currentNode;
                        }

                        currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                    }

                    if(previousValidNode == null)
                    {
                        keyValuePair = default(KeyValuePair<TKey, TValue>);
                        return false;
                    }
                    keyValuePair = new KeyValuePair<TKey, TValue>(previousValidNode.Key, previousValidNode.Value);
                    return true;

                default:
                    throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
            }
        }

        public bool TryGetFromWeight(int weight, out KeyValuePair<TKey, TValue> keyValuePair)
        {
            var localLoopbackNode = loopbackNode;
            TreeNode currentNode;

            if(weight == 0)
            {
                currentNode = localLoopbackNode.Left;
                if(currentNode == localLoopbackNode)
                {
                    keyValuePair = default(KeyValuePair<TKey, TValue>);
                    return false;
                }
                keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                return true;
            }

            currentNode = localLoopbackNode.Parent;

            if(currentNode.Weight <= weight)
            {
                if(currentNode.Weight == weight)
                {
                    currentNode = localLoopbackNode.Right;
                    if(currentNode.Weight != 0)
                    {
                        keyValuePair = default(KeyValuePair<TKey, TValue>);
                        return false;
                    }
                    keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
                    return true;
                }

                keyValuePair = default(KeyValuePair<TKey, TValue>);
                throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
            }

            while(true)
            {
                var nextNode = currentNode.Left;
                var leftWeight = nextNode.Weight;
                if(weight < leftWeight)
                {
                    currentNode = nextNode;
                }
                else
                {
                    weight -= leftWeight;
                    nextNode = currentNode.Right;

                    var weightAtNode = currentNode.Weight - leftWeight - nextNode.Weight;

                    if(weight < weightAtNode)
                    {
                        break;
                    }
                    weight -= weightAtNode;

                    currentNode = nextNode;
                }
            }

            keyValuePair = new KeyValuePair<TKey, TValue>(currentNode.Key, currentNode.Value);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var currentNode = loopbackNode.Parent;
            int comparison;
            while(currentNode != loopbackNode)
            {
                comparison = comparer.Compare(key, currentNode.Key);

                if(comparison == 0)
                {
                    value = currentNode.Value;
                    return true;
                }

                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
            }

            value = default(TValue);
            return false;
        }

        #region explicit IDictionary

        ICollection IDictionary.Keys
        {
            get
            {
                return keys;
            }
        }

        ICollection IDictionary.Values
        {
            get
            {
                return values;
            }
        }

        void IDictionary.Add(object key, object value)
        {
            Add((TKey)key, (TValue)value, 0, false);
        }

        bool IDictionary.Contains(object key)
        {
            return ContainsKey((TKey)key);
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        bool IDictionary.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        bool IDictionary.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IDictionary.Remove(object key)
        {
            Remove((TKey)key);
        }

        #endregion

        #region explicit IDictionary<TKey, TValue>

        ICollection<TKey> IDictionary<TKey, TValue>.Keys
        {
            get
            {
                return keys;
            }
        }

        ICollection<TValue> IDictionary<TKey, TValue>.Values
        {
            get
            {
                return values;
            }
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            Add(key, value, 0, false);
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            return Remove(key);
        }

        #endregion

        # region explicit ICollection

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            CopyTo((KeyValuePair<TKey, TValue>[])array, index);
        }

        #endregion

        #region explicit ICollection<KeyValuePair<TKey, TValue>>

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value, 0, false);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ContainsKey(item.Key);
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        #endregion

        #region explicit IEnumerable

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region explicit IEnumerable<KeyValuePair<TKey, TValue>>

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region private members

        private int Add(TKey key, TValue value, int weight, bool isReplaceable)
        {
            TreeNode newNode;
            if(count == 0)
            {
                if(weight < 0)
                {
                    throw new ArgumentException("weight cannot be negative", "weight");
                }
                newNode = new TreeNode(loopbackNode, loopbackNode, loopbackNode, key, value, false, weight);
                loopbackNode.Parent = newNode;
                loopbackNode.Left = newNode;
                loopbackNode.Right = newNode;
                count = 1;
                return 0;
            }
            TreeNode newParentNode;
            newNode = loopbackNode.Parent;
            int comparison;
            var cumulativeWeight = 0;
            do
            {
                newParentNode = newNode;
                comparison = comparer.Compare(key, newNode.Key);
                newNode = newNode.Left;
                if(comparison >= 0)
                {
                    if(comparison == 0)
                    {
                        if(isReplaceable)
                        {
                            // replace the key incase the key has hidden information not measure by the 
                            // comparer.  It will still be in the right order
                            newParentNode.Key = key;
                            newParentNode.Value = value;

                            // we should only be called with isReplaceable set to true from
                            // the array index operator, which can't set weight, so keep the existing
                            // weight
                            Debug.Assert(weight == 0);
                            return 0;
                        }
                        if(!isAllowedDuplicates)
                        {
                            throw new InvalidOperationException("Key already exists");
                        }
                    }
                    newNode = newParentNode.Right;
                    cumulativeWeight += newParentNode.Weight - newNode.Weight;
                }
            } while(newNode != loopbackNode);

            ++count;
            newNode = new TreeNode(newParentNode, loopbackNode, loopbackNode, key, value, true, weight);

            if(comparison < 0)
            {
                newParentNode.Left = newNode;
                if(newParentNode == loopbackNode.Left)
                {
                    loopbackNode.Left = newNode;
                }
            }
            else
            {
                newParentNode.Right = newNode;
                if(newParentNode == loopbackNode.Right)
                {
                    loopbackNode.Right = newNode;
                }
            }

            TreeNode grandparentNode;
            if(weight != 0)
            {
                if(weight < 0)
                {
                    throw new ArgumentException("weight cannot be negative", "weight");
                }
                for(grandparentNode = newParentNode; grandparentNode != loopbackNode; grandparentNode = grandparentNode.Parent)
                {
                    grandparentNode.Weight += weight;
                }
            }

            while(newParentNode.IsRed)
            {
                grandparentNode = newParentNode.Parent;
                TreeNode uncleNode;
                if(newParentNode == grandparentNode.Right)
                {
                    uncleNode = grandparentNode.Left;
                    if(uncleNode.IsRed)
                    {
                        newParentNode.IsRed = false;
                        uncleNode.IsRed = false;
                        grandparentNode.IsRed = true;

                        newNode = grandparentNode;
                        newParentNode = grandparentNode.Parent;

                        if(newParentNode != loopbackNode)
                        {
                            continue;
                        }

                        newNode.IsRed = false;
                        break;
                    }
                    if(newNode == newParentNode.Left)
                    {
                        RotateRight(newParentNode);
                        newNode = newNode.Right;
                        newParentNode = newNode.Parent;
                    }
                    newParentNode.IsRed = false;
                    grandparentNode.IsRed = true;
                    RotateLeft(grandparentNode);
                    break;
                }
                uncleNode = grandparentNode.Right;
                if(uncleNode.IsRed)
                {
                    newParentNode.IsRed = false;
                    uncleNode.IsRed = false;
                    grandparentNode.IsRed = true;

                    newNode = grandparentNode;
                    newParentNode = grandparentNode.Parent;

                    if(newParentNode != loopbackNode)
                    {
                        continue;
                    }

                    newNode.IsRed = false;
                    break;

                }
                if(newNode == newParentNode.Right)
                {
                    RotateLeft(newParentNode);
                    newNode = newNode.Left;
                    newParentNode = newNode.Parent;
                }

                newParentNode.IsRed = false;
                grandparentNode.IsRed = true;
                RotateRight(grandparentNode);
                break;
            }

            return cumulativeWeight;
        }

        private void AddDictionary(IDictionary<TKey, TValue> dictionary)
        {
            foreach(var keyValuePair in dictionary)
            {
                Add(keyValuePair.Key, keyValuePair.Value);
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
        private void Remove(TreeNode removeNode)
        {
            --count;

            var firstNode = removeNode.Left;
            var secondNode = removeNode.Right;

            var parentNode = removeNode.Parent;
            TreeNode tempNode;

            var changeUppwardWeight = removeNode.Weight - firstNode.Weight - secondNode.Weight;
            if(changeUppwardWeight != 0)
            {
                for(tempNode = parentNode; tempNode != loopbackNode; tempNode = tempNode.Parent)
                {
                    tempNode.Weight -= changeUppwardWeight;
                }
            }

            if(firstNode == loopbackNode)
            {
                firstNode = secondNode;

                if(secondNode != loopbackNode)
                {
                    secondNode.Parent = parentNode;

                    if(loopbackNode.Left == removeNode)
                    {
                        loopbackNode.Left = secondNode;
                    }
                }
                else
                {
                    if(loopbackNode.Right == removeNode)
                    {
                        loopbackNode.Right = parentNode;
                    }

                    if(loopbackNode.Left == removeNode)
                    {
                        loopbackNode.Left = parentNode;
                    }
                }

                if(loopbackNode.Parent == removeNode)
                {
                    loopbackNode.Parent = firstNode;
                }
                else if(parentNode.Right == removeNode)
                {
                    parentNode.Right = firstNode;
                }
                else
                {
                    parentNode.Left = firstNode;
                }

                if(removeNode.IsRed)
                {
                    return;
                }
            }
            else if(secondNode == loopbackNode)
            {
                Debug.Assert(firstNode != loopbackNode);
                firstNode.Parent = parentNode;

                if(loopbackNode.Right == removeNode)
                {
                    loopbackNode.Right = firstNode;
                }

                if(loopbackNode.Parent == removeNode)
                {
                    loopbackNode.Parent = firstNode;
                }
                else if(parentNode.Right == removeNode)
                {
                    parentNode.Right = firstNode;
                }
                else
                {
                    parentNode.Left = firstNode;
                }

                if(removeNode.IsRed)
                {
                    return;
                }
            }
            else
            {
                tempNode = secondNode.Left;
                if(tempNode == loopbackNode)
                {
                    if(loopbackNode.Parent == removeNode)
                    {
                        loopbackNode.Parent = secondNode;
                    }
                    else if(parentNode.Left == removeNode)
                    {
                        parentNode.Left = secondNode;
                    }
                    else
                    {
                        parentNode.Right = secondNode;
                    }

                    firstNode.Parent = secondNode;
                    secondNode.Left = firstNode;
                    secondNode.Weight += firstNode.Weight;

                    firstNode = secondNode.Right;
                    parentNode = secondNode;
                }
                else
                {
                    secondNode = tempNode;
                    while(true)
                    {
                        tempNode = secondNode.Left;

                        if(tempNode == loopbackNode)
                        {
                            break;
                        }

                        secondNode = tempNode;
                    }

                    if(loopbackNode.Parent == removeNode)
                    {
                        loopbackNode.Parent = secondNode;
                    }
                    else if(parentNode.Left == removeNode)
                    {
                        parentNode.Left = secondNode;
                    }
                    else
                    {
                        parentNode.Right = secondNode;
                    }

                    parentNode = secondNode.Parent;

                    changeUppwardWeight = secondNode.Weight - secondNode.Right.Weight;
                    secondNode.Weight = firstNode.Weight + removeNode.Right.Weight;
                    if(changeUppwardWeight != 0)
                    {
                        for(tempNode = parentNode; tempNode != removeNode; tempNode = tempNode.Parent)
                            tempNode.Weight -= changeUppwardWeight;
                    }

                    firstNode.Parent = secondNode;
                    secondNode.Left = firstNode;

                    firstNode = secondNode.Right;

                    if(firstNode != loopbackNode)
                    {
                        firstNode.Parent = parentNode;
                    }
                    tempNode = removeNode.Right;
                    tempNode.Parent = secondNode;
                    parentNode.Left = firstNode;
                    secondNode.Right = tempNode;
                }

                secondNode.Parent = removeNode.Parent;

                if(secondNode.IsRed)
                {
                    secondNode.IsRed = removeNode.IsRed;
                    return;
                }
                secondNode.IsRed = removeNode.IsRed;
            }

            tempNode = loopbackNode.Parent;
            while(firstNode != tempNode && !firstNode.IsRed)
            {
                if(firstNode == parentNode.Right)
                {
                    secondNode = parentNode.Left;
                    if(secondNode.IsRed)
                    {
                        secondNode.IsRed = false;
                        parentNode.IsRed = true;
                        RotateRight(parentNode);
                        secondNode = parentNode.Left;
                    }

                    Debug.Assert(secondNode != loopbackNode);

                    if(secondNode.Left.IsRed)
                        goto left_red;

                    if(!secondNode.Right.IsRed)
                    {
                        secondNode.IsRed = true;
                        firstNode = parentNode;
                        parentNode = firstNode.Parent;
                        continue;
                    }
                    secondNode.IsRed = true;
                    secondNode.Right.IsRed = false;
                    RotateLeft(secondNode);
                    secondNode = parentNode.Left;
                left_red:
                    ;

                    secondNode.IsRed = parentNode.IsRed;
                    secondNode.Left.IsRed = false;
                    parentNode.IsRed = false;
                    RotateRight(parentNode);
                    firstNode.IsRed = false;
                    return;
                }
                secondNode = parentNode.Right;
                if(secondNode.IsRed)
                {
                    secondNode.IsRed = false;
                    parentNode.IsRed = true;
                    RotateLeft(parentNode);
                    secondNode = parentNode.Right;
                }

                Debug.Assert(secondNode != loopbackNode);

                if(secondNode.Right.IsRed)
                    goto right_red;

                if(!secondNode.Left.IsRed)
                {
                    secondNode.IsRed = true;
                    firstNode = parentNode;
                    parentNode = firstNode.Parent;
                    continue;
                }
                secondNode.IsRed = true;
                secondNode.Left.IsRed = false;
                RotateRight(secondNode);
                secondNode = parentNode.Right;
                right_red:
                ;

                secondNode.IsRed = parentNode.IsRed;
                secondNode.Right.IsRed = false;
                parentNode.IsRed = false;
                RotateLeft(parentNode);
                firstNode.IsRed = false;
                return;
            }
            firstNode.IsRed = false;
        }

        private void RotateLeft(TreeNode rotateNode)
        {
            var rightNode = rotateNode.Right;
            var tempNode = rightNode.Left;

            var nodeWeight = rightNode.Weight;
            rightNode.Weight = rotateNode.Weight;
            rotateNode.Weight += tempNode.Weight - nodeWeight;

            rotateNode.Right = tempNode;

            if(tempNode != loopbackNode)
            {
                tempNode.Parent = rotateNode;
            }
            tempNode = rotateNode.Parent;

            rightNode.Parent = tempNode;

            if(rotateNode == loopbackNode.Parent)
            {
                loopbackNode.Parent = rightNode;
            }
            else if(rotateNode == tempNode.Left)
            {
                tempNode.Left = rightNode;
            }
            else
            {
                tempNode.Right = rightNode;
            }
            rightNode.Left = rotateNode;
            rotateNode.Parent = rightNode;
        }

        private void RotateRight(TreeNode rotateNode)
        {
            var leftNode = rotateNode.Left;
            var tempNode = leftNode.Right;

            var nodeWeight = leftNode.Weight;
            leftNode.Weight = rotateNode.Weight;
            rotateNode.Weight += tempNode.Weight - nodeWeight;

            rotateNode.Left = tempNode;

            if(tempNode != loopbackNode)
            {
                tempNode.Parent = rotateNode;
            }
            tempNode = rotateNode.Parent;

            leftNode.Parent = tempNode;

            if(rotateNode == loopbackNode.Parent)
            {
                loopbackNode.Parent = leftNode;
            }
            else if(rotateNode == tempNode.Right)
            {
                tempNode.Right = leftNode;
            }
            else
            {
                tempNode.Left = leftNode;
            }
            leftNode.Right = rotateNode;
            rotateNode.Parent = leftNode;
        }

        #endregion

        #region KeyCollection class

        [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
        public sealed class KeyCollection : ICollection<TKey>, IEnumerable<TKey>, ICollection, IEnumerable
        {
            private readonly TreeDictionary<TKey, TValue> tree;

            internal KeyCollection(TreeDictionary<TKey, TValue> tree)
            {
                this.tree = tree;
            }

            public int Count
            {
                get
                {
                    return tree.Count;
                }
            }

            [SuppressMessage("Microsoft.Usage", "CA2233", Justification = "Potential overflow has been addressed in the header instead of checking at each assignment")]
            public void CopyTo(TKey[] array, int arrayIndex)
            {
                if(array.Length < arrayIndex + Count)
                {
                    throw new ArgumentException("array is not large enough to store this collection", "array");
                }

                var localLoopbackNode = tree.loopbackNode;
                var currentNode = localLoopbackNode.Left;
                TreeNode nextNode;

                if(currentNode == localLoopbackNode)
                {
                    return;
                }

                while(true)
                {
                    array[arrayIndex] = currentNode.Key;
                    ++arrayIndex;
                    nextNode = currentNode.Right;
                    if(nextNode == localLoopbackNode)
                    {
                        if(currentNode != localLoopbackNode.Right)
                        {
                            while(true)
                            {
                                nextNode = currentNode.Parent;
                                if(nextNode.Left != currentNode)
                                {
                                    currentNode = nextNode;
                                    continue;
                                }
                                currentNode = nextNode;
                                break;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        currentNode = nextNode;
                        while(true)
                        {
                            nextNode = currentNode.Left;
                            if(nextNode != localLoopbackNode)
                            {
                                currentNode = nextNode;
                                continue;
                            }
                            break;
                        }
                    }
                }
            }

            public TreeDictionaryKeyEnumerator<TKey, TValue> GetEnumerator()
            {
                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, tree.loopbackNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit);
            }

            public TKey GetFromKey(TKey key, TraversalStartingPoint startingPoint)
            {
                if(TryGetFromKey(key, startingPoint, out key))
                {
                    return key;
                }
                throw new InvalidOperationException("This TreeDictionary does not contain this key");
            }

            public TKey GetFromWeight(int weight)
            {
                TKey outKey;
                if(TryGetFromWeight(weight, out outKey))
                {
                    return outKey;
                }
                throw new InvalidOperationException("This TreeDictionary does not contain this weight");
            }

            public StartFromDirectionCollection StartFromDirection(TraversalDirection direction)
            {
                return new StartFromDirectionCollection(tree, direction);
            }

            public StartFromKeyCollection StartFromKey(TKey key)
            {
                return new StartFromKeyCollection(tree, key, TraversalStartingPoint.EqualOrError, TraversalDirection.LowToHigh);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalDirection direction)
            {
                return new StartFromKeyCollection(tree, key, TraversalStartingPoint.EqualOrError, direction);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint)
            {
                return new StartFromKeyCollection(tree, key, startingPoint, TraversalDirection.LowToHigh);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
            {
                return new StartFromKeyCollection(tree, key, startingPoint, direction);
            }

            public StartFromWeightCollection StartFromWeight(int weight)
            {
                return new StartFromWeightCollection(tree, weight, TraversalDirection.LowToHigh);
            }

            public StartFromWeightCollection StartFromWeight(int weight, TraversalDirection direction)
            {
                return new StartFromWeightCollection(tree, weight, direction);
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
            public bool TryGetFromKey(TKey key, TraversalStartingPoint startingPoint, out TKey outKey)
            {
                TreeNode previousValidNode = null;
                var currentNode = tree.loopbackNode.Parent;
                int comparison;

                switch(startingPoint)
                {
                    case TraversalStartingPoint.EqualOrError:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison == 0)
                            {
                                outKey = currentNode.Key;
                                return true;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        outKey = default(TKey);
                        return false;

                    case TraversalStartingPoint.EqualOrLess:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(0 <= comparison)
                            {
                                if(comparison == 0)
                                {
                                    outKey = currentNode.Key;
                                    return true;
                                }
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            outKey = default(TKey);
                            return false;
                        }
                        outKey = previousValidNode.Key;
                        return true;
                    case TraversalStartingPoint.EqualOrMore:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison <= 0)
                            {
                                if(comparison == 0)
                                {
                                    outKey = currentNode.Key;
                                    return true;
                                }
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            outKey = default(TKey);
                            return false;
                        }
                        outKey = previousValidNode.Key;
                        return true;

                    case TraversalStartingPoint.Less:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(0 < comparison)
                            {
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            outKey = default(TKey);
                            return false;
                        }
                        outKey = previousValidNode.Key;
                        return true;

                    case TraversalStartingPoint.More:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison < 0)
                            {
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            outKey = default(TKey);
                            return false;
                        }
                        outKey = previousValidNode.Key;
                        return true;

                    default:
                        throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                }
            }

            public bool TryGetFromWeight(int weight, out TKey outKey)
            {
                var localLoopbackNode = tree.loopbackNode;
                TreeNode currentNode;

                if(weight == 0)
                {
                    currentNode = localLoopbackNode.Left;
                    if(currentNode == localLoopbackNode)
                    {
                        outKey = default(TKey);
                        return false;
                    }
                    outKey = currentNode.Key;
                    return true;
                }

                currentNode = localLoopbackNode.Parent;

                if(currentNode.Weight <= weight)
                {
                    if(currentNode.Weight == weight)
                    {
                        currentNode = localLoopbackNode.Right;
                        if(currentNode.Weight != 0)
                        {
                            outKey = default(TKey);
                            return false;
                        }
                        outKey = currentNode.Key;
                        return true;
                    }

                    outKey = default(TKey);
                    throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
                }

                while(true)
                {
                    var nextNode = currentNode.Left;
                    var leftWeight = nextNode.Weight;
                    if(weight < leftWeight)
                    {
                        currentNode = nextNode;
                    }
                    else
                    {
                        weight -= leftWeight;
                        nextNode = currentNode.Right;

                        var weightAtNode = currentNode.Weight - leftWeight - nextNode.Weight;

                        if(weight < weightAtNode)
                        {
                            break;
                        }
                        weight -= weightAtNode;

                        currentNode = nextNode;
                    }
                }

                outKey = currentNode.Key;
                return true;
            }

            # region explicit ICollection

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return tree.syncRoot;
                }
            }

            void ICollection.CopyTo(Array array, int index)
            {
                CopyTo((TKey[])array, index);
            }

            #endregion

            #region explicit ICollection<TKey>

            bool ICollection<TKey>.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.KeyCollection.Add(TKey item) is not supported");
            }

            void ICollection<TKey>.Clear()
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.KeyCollection.Clear() is not supported");
            }

            bool ICollection<TKey>.Contains(TKey item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.KeyCollection.Contains(TKey item) is not supported");
            }

            bool ICollection<TKey>.Remove(TKey item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.KeyCollection.Remove(TKey item) is not supported");
            }

            #endregion

            #region explicit IEnumerator

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region explicit IEnumerator<TKey>

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region StartFromKeyCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromKeyCollection : IEnumerable<TKey>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                readonly TKey key;
                readonly TraversalStartingPoint startingPoint;
                readonly TraversalDirection direction;

                internal StartFromKeyCollection(TreeDictionary<TKey, TValue> tree, TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.key = key;
                    this.startingPoint = startingPoint;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
                {
                    if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
                {
                    if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public bool Equals(StartFromKeyCollection startFromKeyCollection)
                {
                    if(tree == startFromKeyCollection.tree && startingPoint == startFromKeyCollection.startingPoint && direction == startFromKeyCollection.direction && tree.comparer.Compare(key, startFromKeyCollection.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromDirectionCollection)
                    {
                        return Equals((StartFromDirectionCollection)obj);
                    }
                    return false;
                }

                [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
                public TreeDictionaryKeyEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromKeyCollection must be initialized before use");
                    }

                    var loopbackNode = tree.loopbackNode;
                    var comparer = tree.comparer;

                    var previousValidNode = loopbackNode;
                    var currentNode = previousValidNode.Parent;
                    int comparison;
                    bool isEqualSeen;

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        switch(startingPoint)
                        {
                            case TraversalStartingPoint.EqualOrError:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison == 0)
                                    {
                                        previousValidNode = currentNode;
                                    }
                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                if(previousValidNode != loopbackNode)
                                {
                                    return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }

                                throw new ArgumentException("There is no item in the tree with the given key", "key");

                            case TraversalStartingPoint.EqualOrLess:
                                isEqualSeen = false;
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 <= comparison)
                                    {
                                        if(comparison == 0)
                                        {
                                            isEqualSeen = true;
                                            previousValidNode = currentNode;
                                        }
                                        else
                                        {
                                            if(!isEqualSeen)
                                            {
                                                previousValidNode = currentNode;
                                            }
                                        }
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.EqualOrMore:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison <= 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.Less:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 < comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.More:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison < 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            default:
                                throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                        }
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        switch(startingPoint)
                        {
                            case TraversalStartingPoint.EqualOrError:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison == 0)
                                    {
                                        previousValidNode = currentNode;
                                    }
                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                if(previousValidNode != loopbackNode)
                                {
                                    return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, true, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }

                                throw new ArgumentException("There is no item in the tree with the given key", "key");

                            case TraversalStartingPoint.EqualOrLess:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 <= comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.EqualOrMore:
                                isEqualSeen = false;
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison <= 0)
                                    {
                                        if(comparison == 0)
                                        {
                                            isEqualSeen = true;
                                            previousValidNode = currentNode;
                                        }
                                        else
                                        {
                                            if(!isEqualSeen)
                                            {
                                                previousValidNode = currentNode;
                                            }
                                        }
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.Less:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 < comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.More:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison < 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                            default:
                                throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                        }
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ startingPoint.GetHashCode() ^ direction.GetHashCode() ^ key.GetHashCode();
                }

                #region explicit IEnumerable<TKey>

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion
            }

            #endregion

            #region StartFromWeightCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromWeightCollection : IEnumerable<TKey>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                int weight;
                readonly TraversalDirection direction;

                internal StartFromWeightCollection(TreeDictionary<TKey, TValue> tree, int weight, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.weight = weight;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
                {
                    if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
                {
                    if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                    {
                        return false;
                    }
                    return true;
                }

                public bool Equals(StartFromWeightCollection startFromWeightCollection)
                {
                    if(tree == startFromWeightCollection.tree && weight == startFromWeightCollection.weight && direction == startFromWeightCollection.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromWeightCollection)
                    {
                        return Equals((StartFromWeightCollection)obj);
                    }
                    return false;
                }

                public TreeDictionaryKeyEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromWeightCollection must be initialized before use");
                    }

                    var loopbackNode = tree.loopbackNode;

                    var previousValidNode = loopbackNode;
                    var currentNode = loopbackNode.Parent;
                    TreeNode nextNode;

                    if(currentNode.Weight < weight)
                    {
                        throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
                    }

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        if(weight == 0)
                        {
                            return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit);
                        }

                        while(true)
                        {
                            nextNode = currentNode.Left;
                            var leftNodeWeight = nextNode.Weight;
                            if(weight <= leftNodeWeight)
                            {
                                previousValidNode = currentNode;
                                currentNode = nextNode;
                            }
                            else
                            {
                                nextNode = currentNode.Right;
                                weight = weight - currentNode.Weight + nextNode.Weight;

                                if(0 < weight)
                                {
                                    currentNode = nextNode;
                                }
                                else
                                {
                                    if(weight == 0)
                                    {
                                        if(nextNode == loopbackNode)
                                        {
                                            return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                                        }
                                        currentNode = nextNode;
                                        while(true)
                                        {
                                            nextNode = currentNode.Left;
                                            if(nextNode != loopbackNode)
                                            {
                                                currentNode = nextNode;
                                            }
                                            else
                                            {
                                                Debug.Assert(currentNode != loopbackNode);
                                                return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                                            }
                                        }
                                    }
                                    Debug.Assert(currentNode != loopbackNode);
                                    return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }
                            }
                        }
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        if(weight == currentNode.Weight)
                        {
                            return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, loopbackNode, true, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit);
                        }

                        weight = currentNode.Weight - weight - 1;
                        while(true)
                        {
                            nextNode = currentNode.Right;
                            var rightWeight = nextNode.Weight;
                            if(weight < rightWeight)
                            {
                                currentNode = nextNode;
                            }
                            else
                            {
                                weight -= rightWeight;
                                nextNode = currentNode.Left;
                                var weightAtNode = currentNode.Weight - rightWeight - nextNode.Weight;

                                if(weight < weightAtNode)
                                {
                                    break;
                                }
                                weight -= weightAtNode;

                                currentNode = nextNode;
                            }
                        }

                        return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, currentNode, true, currentNode == loopbackNode ? TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration);
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ weight.GetHashCode() ^ direction.GetHashCode();
                }

                #region explicit IEnumerable<TKey>

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion
            }

            #endregion

            #region StartFromDirectionCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromDirectionCollection : IEnumerable<TKey>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                readonly TraversalDirection direction;

                internal StartFromDirectionCollection(TreeDictionary<TKey, TValue> tree, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
                {
                    if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
                {
                    if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                    {
                        return false;
                    }
                    return true;
                }

                public bool Equals(StartFromDirectionCollection startFromDirectionCollection)
                {
                    if(tree == startFromDirectionCollection.tree && direction == startFromDirectionCollection.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromDirectionCollection)
                    {
                        return Equals((StartFromDirectionCollection)obj);
                    }
                    return false;
                }

                public TreeDictionaryKeyEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromDirectionCollection must be initialized before use");
                    }

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, tree.loopbackNode, false, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsBeforeLowestBit);
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        return new TreeDictionaryKeyEnumerator<TKey, TValue>(tree, tree.loopbackNode, true, TreeDictionaryKeyEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyEnumerator<TKey, TValue>.IsAfterHighestBit);
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ direction.GetHashCode();
                }

                #region explicit IEnumerable<TKey>

                IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

            }

            #endregion
        }

        #endregion

        #region StartFromKeyCollection

        [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
        public struct StartFromKeyCollection : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            readonly TreeDictionary<TKey, TValue> tree;
            readonly TKey key;
            readonly TraversalStartingPoint startingPoint;
            readonly TraversalDirection direction;

            internal StartFromKeyCollection(TreeDictionary<TKey, TValue> tree, TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
            {
                this.tree = tree;
                this.key = key;
                this.startingPoint = startingPoint;
                this.direction = direction;
            }

            public static bool operator ==(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
            {
                if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                {
                    return true;
                }
                return false;
            }

            public static bool operator !=(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
            {
                if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                {
                    return true;
                }
                return false;
            }

            public bool Equals(StartFromKeyCollection startFromKeyCollection)
            {
                if(tree == startFromKeyCollection.tree && startingPoint == startFromKeyCollection.startingPoint && direction == startFromKeyCollection.direction && tree.comparer.Compare(key, startFromKeyCollection.key) == 0)
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if(obj is StartFromDirectionCollection)
                {
                    return Equals((StartFromDirectionCollection)obj);
                }
                return false;
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
            public TreeDictionaryKeyValuePairEnumerator<TKey, TValue> GetEnumerator()
            {
                if(tree == null)
                {
                    throw new InvalidOperationException("StartFromKeyCollection must be initialized before use");
                }

                var loopbackNode = tree.loopbackNode;
                var comparer = tree.comparer;

                var previousValidNode = loopbackNode;
                var currentNode = loopbackNode.Parent;
                int comparison;
                bool isEqualSeen;

                if(TraversalDirection.LowToHigh == direction)
                {
                    switch(startingPoint)
                    {
                        case TraversalStartingPoint.EqualOrError:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison == 0)
                                {
                                    previousValidNode = currentNode;
                                }
                                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                            }

                            if(previousValidNode != loopbackNode)
                            {
                                return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                            }

                            throw new ArgumentException("There is no item in the tree with the given key", "key");

                        case TraversalStartingPoint.EqualOrLess:
                            isEqualSeen = false;
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(0 <= comparison)
                                {
                                    if(comparison == 0)
                                    {
                                        isEqualSeen = true;
                                        previousValidNode = currentNode;
                                    }
                                    else
                                    {
                                        if(!isEqualSeen)
                                        {
                                            previousValidNode = currentNode;
                                        }
                                    }
                                }

                                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.EqualOrMore:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison <= 0)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.Less:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(0 < comparison)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.More:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison < 0)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        default:
                            throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                    }
                }
                if(TraversalDirection.HighToLow == direction)
                {
                    switch(startingPoint)
                    {
                        case TraversalStartingPoint.EqualOrError:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison == 0)
                                {
                                    previousValidNode = currentNode;
                                }
                                currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                            }

                            if(previousValidNode != loopbackNode)
                            {
                                return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, true, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                            }

                            throw new ArgumentException("There is no item in the tree with the given key", "key");

                        case TraversalStartingPoint.EqualOrLess:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(0 <= comparison)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.EqualOrMore:
                            isEqualSeen = false;
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison <= 0)
                                {
                                    if(comparison == 0)
                                    {
                                        isEqualSeen = true;
                                        previousValidNode = currentNode;
                                    }
                                    else
                                    {
                                        if(!isEqualSeen)
                                        {
                                            previousValidNode = currentNode;
                                        }
                                    }
                                }

                                currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.Less:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(0 < comparison)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        case TraversalStartingPoint.More:
                            while(currentNode != loopbackNode)
                            {
                                comparison = comparer.Compare(key, currentNode.Key);

                                if(comparison < 0)
                                {
                                    previousValidNode = currentNode;
                                }

                                currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                            }

                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                        default:
                            throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                    }
                }
                throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
            }

            public override int GetHashCode()
            {
                return tree.GetHashCode() ^ startingPoint.GetHashCode() ^ direction.GetHashCode() ^ key.GetHashCode();
            }

            #region explicit IEnumerable<KeyValuePair<TKey, TValue>>

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region explicit IEnumerable

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

        }

        #endregion

        #region StartFromWeightCollection

        [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
        public struct StartFromWeightCollection : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            readonly TreeDictionary<TKey, TValue> tree;
            int weight;
            readonly TraversalDirection direction;

            internal StartFromWeightCollection(TreeDictionary<TKey, TValue> tree, int weight, TraversalDirection direction)
            {
                this.tree = tree;
                this.weight = weight;
                this.direction = direction;
            }

            public static bool operator ==(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
            {
                if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                {
                    return true;
                }
                return false;
            }

            public static bool operator !=(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
            {
                if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                {
                    return false;
                }
                return true;
            }

            public bool Equals(StartFromWeightCollection startFromWeightCollection)
            {
                if(tree == startFromWeightCollection.tree && weight == startFromWeightCollection.weight && direction == startFromWeightCollection.direction)
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if(obj is StartFromWeightCollection)
                {
                    return Equals((StartFromWeightCollection)obj);
                }
                return false;
            }

            public TreeDictionaryKeyValuePairEnumerator<TKey, TValue> GetEnumerator()
            {
                if(tree == null)
                {
                    throw new InvalidOperationException("StartFromWeightCollection must be initialized before use");
                }

                var loopbackNode = tree.loopbackNode;

                var previousValidNode = loopbackNode;
                var currentNode = loopbackNode.Parent;
                TreeNode nextNode;

                if(currentNode.Weight < weight)
                {
                    throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
                }

                if(TraversalDirection.LowToHigh == direction)
                {
                    if(weight == 0)
                    {
                        return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit);
                    }

                    while(true)
                    {
                        nextNode = currentNode.Left;
                        var leftNodeWeight = nextNode.Weight;
                        if(weight <= leftNodeWeight)
                        {
                            previousValidNode = currentNode;
                            currentNode = nextNode;
                        }
                        else
                        {
                            nextNode = currentNode.Right;
                            weight = weight - currentNode.Weight + nextNode.Weight;

                            if(0 < weight)
                            {
                                currentNode = nextNode;
                            }
                            else
                            {
                                if(weight == 0)
                                {
                                    if(nextNode == loopbackNode)
                                    {
                                        return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                                    }
                                    currentNode = nextNode;
                                    while(true)
                                    {
                                        nextNode = currentNode.Left;
                                        if(nextNode != loopbackNode)
                                        {
                                            currentNode = nextNode;
                                        }
                                        else
                                        {
                                            Debug.Assert(currentNode != loopbackNode);
                                            return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                                        }
                                    }
                                }
                                Debug.Assert(currentNode != loopbackNode);
                                return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                            }
                        }
                    }
                }
                if(TraversalDirection.HighToLow == direction)
                {
                    if(weight == currentNode.Weight)
                    {
                        return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, loopbackNode, true, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit);
                    }

                    weight = currentNode.Weight - weight - 1;
                    while(true)
                    {
                        nextNode = currentNode.Right;
                        var rightWeight = nextNode.Weight;
                        if(weight < rightWeight)
                        {
                            currentNode = nextNode;
                        }
                        else
                        {
                            weight -= rightWeight;
                            nextNode = currentNode.Left;
                            var weightAtNode = currentNode.Weight - rightWeight - nextNode.Weight;

                            if(weight < weightAtNode)
                            {
                                break;
                            }
                            weight -= weightAtNode;

                            currentNode = nextNode;
                        }
                    }

                    return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, currentNode, true, currentNode == loopbackNode ? TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration);
                }
                throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
            }

            public override int GetHashCode()
            {
                return tree.GetHashCode() ^ weight.GetHashCode() ^ direction.GetHashCode();
            }

            #region explicit IEnumerable<KeyValuePair<TKey, TValue>>

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region explicit IEnumerable

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

        }

        #endregion

        #region StartFromDirectionCollection

        [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
        public struct StartFromDirectionCollection : IEnumerable<KeyValuePair<TKey, TValue>>
        {
            readonly TreeDictionary<TKey, TValue> tree;
            readonly TraversalDirection direction;

            internal StartFromDirectionCollection(TreeDictionary<TKey, TValue> tree, TraversalDirection direction)
            {
                this.tree = tree;
                this.direction = direction;
            }

            public static bool operator ==(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
            {
                if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                {
                    return true;
                }
                return false;
            }

            public static bool operator !=(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
            {
                if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                {
                    return false;
                }
                return true;
            }

            public bool Equals(StartFromDirectionCollection startFromDirectionCollection)
            {
                if(tree == startFromDirectionCollection.tree && direction == startFromDirectionCollection.direction)
                {
                    return true;
                }
                return false;
            }

            public override bool Equals(object obj)
            {
                if(obj is StartFromDirectionCollection)
                {
                    return Equals((StartFromDirectionCollection)obj);
                }
                return false;
            }

            public TreeDictionaryKeyValuePairEnumerator<TKey, TValue> GetEnumerator()
            {
                if(tree == null)
                {
                    throw new InvalidOperationException("StartFromDirectionCollection must be initialized before use");
                }

                if(TraversalDirection.LowToHigh == direction)
                {
                    return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, tree.loopbackNode, false, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsBeforeLowestBit);
                }
                if(TraversalDirection.HighToLow == direction)
                {
                    return new TreeDictionaryKeyValuePairEnumerator<TKey, TValue>(tree, tree.loopbackNode, true, TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryKeyValuePairEnumerator<TKey, TValue>.IsAfterHighestBit);
                }
                throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
            }

            public override int GetHashCode()
            {
                return tree.GetHashCode() ^ direction.GetHashCode();
            }

            #region explicit IEnumerable<KeyValuePair<TKey, TValue>>

            IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region explicit IEnumerable

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

        }

        #endregion

        #region TreeNode class

        internal sealed class TreeNode
        {
            internal TreeNode Parent;
            internal TreeNode Left;
            internal TreeNode Right;
            internal TKey Key;
            internal TValue Value;
            internal bool IsRed;
            internal int Weight;

            internal TreeNode()
            {
            }

            internal TreeNode(TreeNode parent, TreeNode left, TreeNode right, TKey key, TValue value, bool isRed, int weight)
            {
                Parent = parent;
                Left = left;
                Right = right;
                Key = key;
                Value = value;
                IsRed = isRed;
                Weight = weight;
            }
        }

        #endregion

        #region ValueCollection class

        [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
        public sealed class ValueCollection : ICollection<TValue>, IEnumerable<TValue>, ICollection, IEnumerable
        {
            private readonly TreeDictionary<TKey, TValue> tree;

            internal ValueCollection(TreeDictionary<TKey, TValue> tree)
            {
                this.tree = tree;
            }

            public int Count
            {
                get
                {
                    return tree.Count;
                }
            }

            [SuppressMessage("Microsoft.Usage", "CA2233", Justification = "Potential overflow has been addressed in the header instead of checking at each assignment")]
            public void CopyTo(TValue[] array, int arrayIndex)
            {
                if(array.Length < arrayIndex + Count)
                {
                    throw new ArgumentException("array is not large enough to store this collection", "array");
                }

                var localLoopbackNode = tree.loopbackNode;
                var currentNode = localLoopbackNode.Left;
                TreeNode nextNode;

                if(currentNode == localLoopbackNode)
                {
                    return;
                }

                while(true)
                {
                    array[arrayIndex] = currentNode.Value;
                    ++arrayIndex;
                    nextNode = currentNode.Right;
                    if(nextNode == localLoopbackNode)
                    {
                        if(currentNode != localLoopbackNode.Right)
                        {
                            while(true)
                            {
                                nextNode = currentNode.Parent;
                                if(nextNode.Left != currentNode)
                                {
                                    currentNode = nextNode;
                                    continue;
                                }
                                currentNode = nextNode;
                                break;
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        currentNode = nextNode;
                        while(true)
                        {
                            nextNode = currentNode.Left;
                            if(nextNode != localLoopbackNode)
                            {
                                currentNode = nextNode;
                                continue;
                            }
                            break;
                        }
                    }
                }
            }

            public TreeDictionaryValueEnumerator<TKey, TValue> GetEnumerator()
            {
                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, tree.loopbackNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit);
            }

            public TValue GetFromKey(TKey key, TraversalStartingPoint startingPoint)
            {
                TValue value;
                if(TryGetFromKey(key, startingPoint, out value))
                {
                    return value;
                }
                throw new InvalidOperationException("This TreeDictionary does not contain this key");
            }

            public TValue GetFromWeight(int weight)
            {
                TValue value;
                if(TryGetFromWeight(weight, out value))
                {
                    return value;
                }
                throw new InvalidOperationException("This TreeDictionary does not contain this weight");
            }

            public StartFromDirectionCollection StartFromDirection(TraversalDirection direction)
            {
                return new StartFromDirectionCollection(tree, direction);
            }

            public StartFromKeyCollection StartFromKey(TKey key)
            {
                return new StartFromKeyCollection(tree, key, TraversalStartingPoint.EqualOrError, TraversalDirection.LowToHigh);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalDirection direction)
            {
                return new StartFromKeyCollection(tree, key, TraversalStartingPoint.EqualOrError, direction);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint)
            {
                return new StartFromKeyCollection(tree, key, startingPoint, TraversalDirection.LowToHigh);
            }

            public StartFromKeyCollection StartFromKey(TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
            {
                return new StartFromKeyCollection(tree, key, startingPoint, direction);
            }

            public StartFromWeightCollection StartFromWeight(int weight)
            {
                return new StartFromWeightCollection(tree, weight, TraversalDirection.LowToHigh);
            }

            public StartFromWeightCollection StartFromWeight(int weight, TraversalDirection direction)
            {
                return new StartFromWeightCollection(tree, weight, direction);
            }

            [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
            public bool TryGetFromKey(TKey key, TraversalStartingPoint startingPoint, out TValue value)
            {
                TreeNode previousValidNode = null;
                var currentNode = tree.loopbackNode.Parent;
                int comparison;

                switch(startingPoint)
                {
                    case TraversalStartingPoint.EqualOrError:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison == 0)
                            {
                                value = currentNode.Value;
                                return true;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        value = default(TValue);
                        return false;

                    case TraversalStartingPoint.EqualOrLess:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(0 <= comparison)
                            {
                                if(comparison == 0)
                                {
                                    value = currentNode.Value;
                                    return true;
                                }
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            value = default(TValue);
                            return false;
                        }
                        value = previousValidNode.Value;
                        return true;
                    case TraversalStartingPoint.EqualOrMore:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison <= 0)
                            {
                                if(comparison == 0)
                                {
                                    value = currentNode.Value;
                                    return true;
                                }
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            value = default(TValue);
                            return false;
                        }
                        value = previousValidNode.Value;
                        return true;

                    case TraversalStartingPoint.Less:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(0 < comparison)
                            {
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            value = default(TValue);
                            return false;
                        }
                        value = previousValidNode.Value;
                        return true;

                    case TraversalStartingPoint.More:
                        while(currentNode != tree.loopbackNode)
                        {
                            comparison = tree.comparer.Compare(key, currentNode.Key);

                            if(comparison < 0)
                            {
                                previousValidNode = currentNode;
                            }

                            currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                        }

                        if(previousValidNode == null)
                        {
                            value = default(TValue);
                            return false;
                        }
                        value = previousValidNode.Value;
                        return true;

                    default:
                        throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                }
            }

            public bool TryGetFromWeight(int weight, out TValue value)
            {
                var localLoopbackNode = tree.loopbackNode;
                TreeNode currentNode;

                if(weight == 0)
                {
                    currentNode = localLoopbackNode.Left;
                    if(currentNode == localLoopbackNode)
                    {
                        value = default(TValue);
                        return false;
                    }
                    value = currentNode.Value;
                    return true;
                }

                currentNode = localLoopbackNode.Parent;

                if(currentNode.Weight <= weight)
                {
                    if(currentNode.Weight == weight)
                    {
                        currentNode = localLoopbackNode.Right;
                        if(currentNode.Weight != 0)
                        {
                            value = default(TValue);
                            return false;
                        }
                        value = currentNode.Value;
                        return true;
                    }

                    value = default(TValue);
                    throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
                }

                while(true)
                {
                    var nextNode = currentNode.Left;
                    var leftWeight = nextNode.Weight;
                    if(weight < leftWeight)
                    {
                        currentNode = nextNode;
                    }
                    else
                    {
                        weight -= leftWeight;
                        nextNode = currentNode.Right;

                        var weightAtNode = currentNode.Weight - leftWeight - nextNode.Weight;

                        if(weight < weightAtNode)
                        {
                            break;
                        }
                        weight -= weightAtNode;

                        currentNode = nextNode;
                    }
                }

                value = currentNode.Value;
                return true;
            }

            # region explicit ICollection

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return tree.syncRoot;
                }
            }

            void ICollection.CopyTo(Array array, int index)
            {
                CopyTo((TValue[])array, index);
            }

            #endregion

            #region explicit ICollection<TValue>

            bool ICollection<TValue>.IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            void ICollection<TValue>.Add(TValue item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.ValueCollection.Add(TValue item) is not supported");
            }

            void ICollection<TValue>.Clear()
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.ValueCollection.Clear() is not supported");
            }

            bool ICollection<TValue>.Contains(TValue item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.ValueCollection.Contains(TValue item) is not supported");
            }

            bool ICollection<TValue>.Remove(TValue item)
            {
                throw new NotSupportedException("void TreeDictionary<TKey, TValue>.ValueCollection.Remove(TValue item) is not supported");
            }

            #endregion

            #region explicit IEnumerator

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region explicit IEnumerator<TValue>

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return GetEnumerator();
            }

            #endregion

            #region StartFromKeyCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromKeyCollection : IEnumerable<TValue>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                readonly TKey key;
                readonly TraversalStartingPoint startingPoint;
                readonly TraversalDirection direction;

                internal StartFromKeyCollection(TreeDictionary<TKey, TValue> tree, TKey key, TraversalStartingPoint startingPoint, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.key = key;
                    this.startingPoint = startingPoint;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
                {
                    if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromKeyCollection startFromKeyCollection1, StartFromKeyCollection startFromKeyCollection2)
                {
                    if(startFromKeyCollection1.tree == startFromKeyCollection2.tree && startFromKeyCollection1.startingPoint == startFromKeyCollection2.startingPoint && startFromKeyCollection1.direction == startFromKeyCollection2.direction && startFromKeyCollection1.tree.comparer.Compare(startFromKeyCollection1.key, startFromKeyCollection2.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public bool Equals(StartFromKeyCollection startFromKeyCollection)
                {
                    if(tree == startFromKeyCollection.tree && startingPoint == startFromKeyCollection.startingPoint && direction == startFromKeyCollection.direction && tree.comparer.Compare(key, startFromKeyCollection.key) == 0)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromDirectionCollection)
                    {
                        return Equals((StartFromDirectionCollection)obj);
                    }
                    return false;
                }

                [SuppressMessage("Microsoft.Maintainability", "CA1502", Justification = "Complex for speed")]
                public TreeDictionaryValueEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromKeyCollection must be initialized before use");
                    }

                    var loopbackNode = tree.loopbackNode;
                    var comparer = tree.comparer;

                    var previousValidNode = loopbackNode;
                    var currentNode = previousValidNode.Parent;
                    int comparison;
                    bool isEqualSeen;

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        switch(startingPoint)
                        {
                            case TraversalStartingPoint.EqualOrError:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison == 0)
                                    {
                                        previousValidNode = currentNode;
                                    }
                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                if(previousValidNode != loopbackNode)
                                {
                                    return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }

                                throw new ArgumentException("There is no item in the tree with the given key", "key");

                            case TraversalStartingPoint.EqualOrLess:
                                isEqualSeen = false;
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 <= comparison)
                                    {
                                        if(comparison == 0)
                                        {
                                            isEqualSeen = true;
                                            previousValidNode = currentNode;
                                        }
                                        else
                                        {
                                            if(!isEqualSeen)
                                            {
                                                previousValidNode = currentNode;
                                            }
                                        }
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.EqualOrMore:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison <= 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.Less:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 < comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.More:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison < 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            default:
                                throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                        }
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        switch(startingPoint)
                        {
                            case TraversalStartingPoint.EqualOrError:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison == 0)
                                    {
                                        previousValidNode = currentNode;
                                    }
                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                if(previousValidNode != loopbackNode)
                                {
                                    return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, true, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }

                                throw new ArgumentException("There is no item in the tree with the given key", "key");

                            case TraversalStartingPoint.EqualOrLess:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 <= comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.EqualOrMore:
                                isEqualSeen = false;
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison <= 0)
                                    {
                                        if(comparison == 0)
                                        {
                                            isEqualSeen = true;
                                            previousValidNode = currentNode;
                                        }
                                        else
                                        {
                                            if(!isEqualSeen)
                                            {
                                                previousValidNode = currentNode;
                                            }
                                        }
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.Less:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(0 < comparison)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison <= 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            case TraversalStartingPoint.More:
                                while(currentNode != loopbackNode)
                                {
                                    comparison = comparer.Compare(key, currentNode.Key);

                                    if(comparison < 0)
                                    {
                                        previousValidNode = currentNode;
                                    }

                                    currentNode = comparison < 0 ? currentNode.Left : currentNode.Right;
                                }

                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, true, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                            default:
                                throw new ArgumentException("startingPoint must a value from the TraversalStartingPoint enumeration", "startingPoint");
                        }
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ startingPoint.GetHashCode() ^ direction.GetHashCode() ^ key.GetHashCode();
                }

                #region explicit IEnumerable<TValue>

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

            }

            #endregion

            #region StartFromWeightCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromWeightCollection : IEnumerable<TValue>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                int weight;
                readonly TraversalDirection direction;

                internal StartFromWeightCollection(TreeDictionary<TKey, TValue> tree, int weight, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.weight = weight;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
                {
                    if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromWeightCollection startFromWeightCollection1, StartFromWeightCollection startFromWeightCollection2)
                {
                    if(startFromWeightCollection1.tree == startFromWeightCollection2.tree && startFromWeightCollection1.weight == startFromWeightCollection2.weight && startFromWeightCollection1.direction == startFromWeightCollection2.direction)
                    {
                        return false;
                    }
                    return true;
                }

                public bool Equals(StartFromWeightCollection startFromWeightCollection)
                {
                    if(tree == startFromWeightCollection.tree && weight == startFromWeightCollection.weight && direction == startFromWeightCollection.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromWeightCollection)
                    {
                        return Equals((StartFromWeightCollection)obj);
                    }
                    return false;
                }

                public TreeDictionaryValueEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromWeightCollection must be initialized before use");
                    }

                    var loopbackNode = tree.loopbackNode;

                    var previousValidNode = loopbackNode;
                    var currentNode = loopbackNode.Parent;
                    TreeNode nextNode;

                    if(currentNode.Weight < weight)
                    {
                        throw new InvalidOperationException("The starting weight is beyond the end of the tree's weights");
                    }

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        if(weight == 0)
                        {
                            return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit);
                        }

                        while(true)
                        {
                            nextNode = currentNode.Left;
                            var leftNodeWeight = nextNode.Weight;
                            if(weight <= leftNodeWeight)
                            {
                                previousValidNode = currentNode;
                                currentNode = nextNode;
                            }
                            else
                            {
                                nextNode = currentNode.Right;
                                weight = weight - currentNode.Weight + nextNode.Weight;

                                if(0 < weight)
                                {
                                    currentNode = nextNode;
                                }
                                else
                                {
                                    if(weight == 0)
                                    {
                                        if(nextNode == loopbackNode)
                                        {
                                            return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, previousValidNode, false, previousValidNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                                        }
                                        currentNode = nextNode;
                                        while(true)
                                        {
                                            nextNode = currentNode.Left;
                                            if(nextNode != loopbackNode)
                                            {
                                                currentNode = nextNode;
                                            }
                                            else
                                            {
                                                Debug.Assert(currentNode != loopbackNode);
                                                return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                                            }
                                        }
                                    }
                                    Debug.Assert(currentNode != loopbackNode);
                                    return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, currentNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                                }
                            }
                        }
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        if(weight == currentNode.Weight)
                        {
                            return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, loopbackNode, true, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit);
                        }

                        weight = currentNode.Weight - weight - 1;
                        while(true)
                        {
                            nextNode = currentNode.Right;
                            var rightWeight = nextNode.Weight;
                            if(weight < rightWeight)
                            {
                                currentNode = nextNode;
                            }
                            else
                            {
                                weight -= rightWeight;
                                nextNode = currentNode.Left;
                                var weightAtNode = currentNode.Weight - rightWeight - nextNode.Weight;

                                if(weight < weightAtNode)
                                {
                                    break;
                                }
                                weight -= weightAtNode;

                                currentNode = nextNode;
                            }
                        }

                        return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, currentNode, true, currentNode == loopbackNode ? TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit : TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration);
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ weight.GetHashCode() ^ direction.GetHashCode();
                }


                #region explicit IEnumerable<TValue>

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion
            }

            #endregion

            #region StartFromDirectionCollection

            [SuppressMessage("Microsoft.Design", "CA1034", Justification = "This is the standard design for similar collections like the SortedDictionary class")]
            public struct StartFromDirectionCollection : IEnumerable<TValue>
            {
                readonly TreeDictionary<TKey, TValue> tree;
                readonly TraversalDirection direction;

                internal StartFromDirectionCollection(TreeDictionary<TKey, TValue> tree, TraversalDirection direction)
                {
                    this.tree = tree;
                    this.direction = direction;
                }

                public static bool operator ==(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
                {
                    if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public static bool operator !=(StartFromDirectionCollection startFromDirectionCollection1, StartFromDirectionCollection startFromDirectionCollection2)
                {
                    if(startFromDirectionCollection1.tree == startFromDirectionCollection2.tree && startFromDirectionCollection1.direction == startFromDirectionCollection2.direction)
                    {
                        return false;
                    }
                    return true;
                }

                public bool Equals(StartFromDirectionCollection startFromDirectionCollection)
                {
                    if(tree == startFromDirectionCollection.tree && direction == startFromDirectionCollection.direction)
                    {
                        return true;
                    }
                    return false;
                }

                public override bool Equals(object obj)
                {
                    if(obj is StartFromDirectionCollection)
                    {
                        return Equals((StartFromDirectionCollection)obj);
                    }
                    return false;
                }

                public TreeDictionaryValueEnumerator<TKey, TValue> GetEnumerator()
                {
                    if(tree == null)
                    {
                        throw new InvalidOperationException("StartFromDirectionCollection must be initialized before use");
                    }

                    if(TraversalDirection.LowToHigh == direction)
                    {
                        return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, tree.loopbackNode, false, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsBeforeLowestBit);
                    }
                    if(TraversalDirection.HighToLow == direction)
                    {
                        return new TreeDictionaryValueEnumerator<TKey, TValue>(tree, tree.loopbackNode, true, TreeDictionaryValueEnumerator<TKey, TValue>.IsStartingEnumeration | TreeDictionaryValueEnumerator<TKey, TValue>.IsAfterHighestBit);
                    }
                    throw new ArgumentException("direction must either be TraversalDirection.LowToHigh or TraversalDirection.HighToLow", "direction");
                }

                public override int GetHashCode()
                {
                    return tree.GetHashCode() ^ direction.GetHashCode();
                }

                #region explicit IEnumerable<TValue>

                IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

                #region explicit IEnumerable

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }

                #endregion

            }

            #endregion
        }

        #endregion

        #region debugging functions

#if DEBUG

        public void VerifyTree(bool checkWeights) {
            Debug.Assert(!loopbackNode.Parent.IsRed);
            Debug.Assert(!loopbackNode.IsRed);
            Debug.Assert(loopbackNode.Parent.Parent == loopbackNode);

            TreeNode leftMost;
            for(leftMost = loopbackNode.Parent; leftMost.Left != loopbackNode; leftMost = leftMost.Left) {
                ;
            }
            Debug.Assert(loopbackNode.Left == leftMost);

            TreeNode rightMost;
            for(rightMost = loopbackNode.Parent; rightMost.Right != loopbackNode; rightMost = rightMost.Right) {
                ;
            }
            Debug.Assert(loopbackNode.Right == rightMost);

            var blackDepth = 0;
            var node = loopbackNode.Parent;
            while(node != loopbackNode) {
                if(!node.IsRed)
                    blackDepth++;
                node = node.Left;
            }

            var minDepth = int.MaxValue;
            var maxDepth = 0;
            var verifyCount = 0;
            VerifyTraverseTree(loopbackNode, loopbackNode.Parent, 1, blackDepth, ref maxDepth, ref minDepth, ref verifyCount, checkWeights);

            Debug.Assert(maxDepth <= minDepth * 2);
            Debug.Assert(count == verifyCount);
        }

        private void VerifyTraverseTree(TreeNode parent, TreeNode node, int depth, int blackDepth, ref int maxDepth, ref int minDepth, ref int verifyCount, bool checkWeights) {
            if(node == loopbackNode) {
                Debug.Assert(0 == blackDepth);
                if(depth < minDepth) {
                    minDepth = depth;
                }
                return;
            }

            if(checkWeights) {
                Debug.Assert(node.Weight - node.Left.Weight - node.Right.Weight == (int)(object)node.Key);
            }
            Debug.Assert(parent == node.Parent);
            if(node.IsRed) {
                Debug.Assert(!node.Left.IsRed);
                Debug.Assert(!node.Right.IsRed);
            } else {
                --blackDepth;
            }

            ++verifyCount;
            if(depth > maxDepth) {
                maxDepth = depth;
            }

            VerifyTraverseTree(node, node.Left, depth + 1, blackDepth, ref maxDepth, ref minDepth, ref verifyCount, checkWeights);
            VerifyTraverseTree(node, node.Right, depth + 1, blackDepth, ref maxDepth, ref minDepth, ref verifyCount, checkWeights);
        }
#endif

        #endregion
    }
}
