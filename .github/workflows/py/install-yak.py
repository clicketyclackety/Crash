import subprocess as proc
from glob import glob
import argparse
import os
import sys

parser = argparse.ArgumentParser()
parser.add_argument("-v", "--version", type=str, default='7', choices=['7', '8 WIP'])

args = parser.parse_args()

yak = f'C:\Program Files\Rhino {args.version}\System\Yak.exe'

# Could use a local package
proc.run( [yak, 'install', 'crash'] )

sys.exit(0)