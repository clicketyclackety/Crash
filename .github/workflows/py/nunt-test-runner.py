import subprocess as proc
from glob import glob
import argparse
import os
import sys

parser = argparse.ArgumentParser()
parser.add_argument("-v", "--version", type=str, default='7', choices=['7', '8 WIP'])
parser.add_argument("-d", "--dll", type=str, required=True)

args = parser.parse_args()

rhino =  f'C:\Program Files\Rhino {args.version}\System\Rhino.exe'
script = f'NUnitTestRunner {args.dll}'
args = f'/nosplash /notemplate /runsript="{script}"'

proc.run( [rhino, args] )

sys.exit(0)