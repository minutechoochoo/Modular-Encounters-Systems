﻿using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRageMath;

namespace ModularEncountersSystems.Spawning.Procedural {

	public static partial class BuilderTools {

		public static Vector3I IncrementX1 = new Vector3I(1,0,0);
		public static Vector3I IncrementZ1 = new Vector3I(0, 0, 1);
		public static Vector3I IncrementX1Z1 = new Vector3I(1, 0, 1);

		public static void BuildStraightArmorLine(ShipConstruct construct, BlockCategory category, Vector3I block, int steps, Vector3I increment, Vector3I endIncrement, bool xsymm, bool ysymm, ref Vector3I currentPosition) {

			currentPosition = BuildLine(construct, category, block, increment, steps, currentPosition, xsymm, ysymm);
			currentPosition += endIncrement;

		}

		public static void BuildStraightStackedArmorLine(ShipConstruct construct, BlockCategory category, int steps, Vector3I increment, Vector3I stackIncrement, Vector3I endIncrement, bool xsymm, bool ysymm, ref Vector3I currentPosition, params Vector3I[] blocks) {

			currentPosition = BuildStackedBlocksLine(construct, category, increment, stackIncrement, steps, currentPosition, xsymm, ysymm, -1, -1, blocks);
			currentPosition += endIncrement;

		}

		public static void BuildDualBlockLine(ShipConstruct construct, BlockCategory category, int steps, int secondStepAdd, Vector3I increment, Vector3I secondLineOffset, Vector3I endIncrement, Vector3I singleStepEndIncrement, Vector3I firstBlock, Vector3I secondBlock, bool xsymm, bool ysymm, ref Vector3I currentPosition) {

			construct.Log.Append("BuildDualBlockLine: Steps: ").Append(steps.ToString()).AppendLine();
			var firstPos = BuildLine(construct, BlockCategory.Armor, firstBlock, increment, steps, currentPosition, xsymm, ysymm);

			if (steps > 1) {

				var secondPos = BuildLine(construct, BlockCategory.Armor, secondBlock, increment, (steps += secondStepAdd), currentPosition += secondLineOffset, xsymm, ysymm);
				currentPosition = secondPos + endIncrement;

			} else {

				currentPosition = firstPos + singleStepEndIncrement;

			}
			
		}

		public static void BuildDualStackedBlockLine(ShipConstruct construct, BlockCategory category, int steps, int secondStepAdd, Vector3I increment, Vector3I stackIncrement, Vector3I secondLineOffset, Vector3I endIncrement, Vector3I singleStepEndIncrement, bool xsymm, bool ysymm, int stackLength, ref Vector3I currentPosition, params Vector3I[] blocks) {

			if (blocks == null || blocks.Length == 0 || blocks.Length % stackLength != 0) {

				//Todo: Raise error about odd amount of blocks provided
				return;
			
			}

			int halfArrayLength = blocks.Length / 2;

			var firstPos = BuildStackedBlocksLine(construct, BlockCategory.Armor, increment, stackIncrement, steps, currentPosition, xsymm, ysymm, 0, halfArrayLength - 1, blocks);

			if (steps > 1) {

				var secondPos = BuildStackedBlocksLine(construct, BlockCategory.Armor, increment, stackIncrement, (steps += secondStepAdd), (currentPosition += secondLineOffset), xsymm, ysymm, halfArrayLength, blocks.Length - 1, blocks);
				currentPosition = secondPos + endIncrement;

			} else {

				currentPosition = firstPos + singleStepEndIncrement;

			}
			

		}

		private static void BuildBlockStackList(List<Vector3I> targetList, Vector3I[] blocks, int stackLength, bool firstLine = true,  bool firstHalf = true) {

			targetList.Clear();

			if (stackLength % 2 != 0)
				return;

			/*
			0 1
			1 2

			2 3
			3 4



			4 5
			5 6

			6 7
			7 8
			*/

			incorp firstline
			int start = firstHalf ? 0 : blocks.Length / 2;
			int end = firstHalf ? blocks.Length / 2 : stackLength;

			for (int i = start; i < end; i++) {

				targetList.Add(blocks[i])


			}

		}

		public static Vector3I BuildLine(ShipConstruct construct, BlockCategory category, Vector3I block, Vector3I increment, int steps, Vector3I position, bool xSymmetry = false, bool ySymmetry = false) {

			int stepsTaken = 0;
			var pos = position;

			while (true) {

				construct.PlaceBlock(category, pos, pos, block, xSymmetry, ySymmetry);

				stepsTaken++;

				if (stepsTaken >= steps)
					break;

				pos += increment;
			
			}

			return pos;
		
		}

		public static void BuildStackedBlocks(ShipConstruct construct, BlockCategory category, Vector3I increment, Vector3I currentPosition, bool xSymmetry, bool ySymmetry, params Vector3I[] blocks) {

			if (blocks == null || blocks.Length == 0)
				return;

			var pos = currentPosition;

			foreach (var block in blocks) {

				construct.PlaceBlock(category, pos, pos, block, xSymmetry, ySymmetry);
				pos += increment;

			}

		}

		public static Vector3I BuildStackedBlocksLine(ShipConstruct construct, BlockCategory category, Vector3I increment, Vector3I stackIncrement, int steps, Vector3I position, bool xSymmetry, bool ySymmetry, List<Vector3I> blocks) {

			var pos = position;
			var stepsTaken = 0;

			while (true) {

				var cellPos = pos;

				for (int i = 0; i < blocks.Count; i++) {

					//Place
					construct.PlaceBlock(category, pos, pos, blocks[i], xSymmetry, ySymmetry);
					cellPos += stackIncrement;
				
				}

				if (stepsTaken >= steps)
					break;

				pos += increment;
			
			}

			return pos;

		}

		public static int CheckLine(ShipConstruct construct, BlockCategory category, Vector3I block, Vector3I increment, Vector3I position, bool matchAnyBlock = false) {

			int hits = 0;
			var currentPosition = position;
			var refBlock = construct.GetReferenceBlock(category, block);

			if (refBlock == null)
				return hits;

			for (int i = 0; i < 250; i++) {

				var cubeBlock = construct.GetBlock(currentPosition);

				if (cubeBlock == null)
					break;

				if(!matchAnyBlock)
					if (cubeBlock.GetId() != refBlock.GetId())
						break;

				currentPosition += increment;
				hits += 1;

			}

			return hits;
		
		}

	}

}
