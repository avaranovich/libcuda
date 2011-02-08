@echo off
call generate_ptx 64 sm_13 ..\matmul.cu
call generate_ptx 64 sm_13 ..\matmul_fast.cu
@pause