#!/usr/bin/env sh

mkdir -p build/gcc
gcc -O3 -o build/gcc/almost_pseudo_random src/main/c/almost_pseudo_random.c -lm
gcc -O3 -o build/gcc/java_2_times_faster_than_c src/main/c/java_2_times_faster_than_c.c -lm
gcc -O3 -o build/gcc/xorshift_rng src/main/c/xorshift_rng.c
gcc -O3 -o build/gcc/java_2_times_faster_than_c_xorshift src/main/c/java_2_times_faster_than_c_xorshift.c
