#include "cuda_runtime.h"
#include "device_launch_parameters.h"
#include "MinutiaStructs.cuh"
#include "MinutiaHelper.cuh"

#include <Math.h>
#include <stdio.h>


__global__ void buildDescriptors(Minutia *mins, int pitch, int *minutiaNum, Descriptor *desc, int dbSize)
{
	int i, j, temp_j, num;
	float length;
	__shared__ int k;
	num = blockIdx.x;
	i = blockIdx.y;
	j = threadIdx.x;
	if (j == 0)
	{
		desc[num*pitch + i].length = 0;
	}
	__syncthreads();

	if (blockIdx.x < dbSize && i < minutiaNum[num] && j < minutiaNum[num])
	{
		length = sqrLength(mins[num*pitch + i], mins[num*pitch + j]);
		if (i != j && length <= DESCRIPTOR_RADIUS*DESCRIPTOR_RADIUS)
		{
			temp_j = atomicAdd(&(desc[num*pitch + i].length), 1);
			desc[num*pitch + i].center = mins[num*pitch + i];
			desc[num*pitch + i].minutias[temp_j] = mins[num*pitch + j];
			//atomicAdd(&(desc[num*pitch + i].length), 1);
		}
	}/*
	__syncthreads();
	if (num == 0 && j == 0 && i == 0)
	{
		for (i = 0; i < minutiaNum[0]; i++)
		{
			printf("desc num %d, desc length %d\n", i, desc[i].length);
			for (j = 0; j < desc[i].length; j++)
			{
				printf("%d %d %f\n", desc[i].minutias[j].x, desc[i].minutias[j].y, desc[i].minutias[j].angle);
			}
		}
	}*/
}