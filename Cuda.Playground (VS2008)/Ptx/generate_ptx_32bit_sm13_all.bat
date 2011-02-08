@echo off
call generate_ptx 32 sm_13 ..\matmul.cu
call generate_ptx 32 sm_13 ..\matmul_fast.cu
@pause