import subprocess as proc
from glob import glob
import argparse
import os
import sys

parser = argparse.ArgumentParser()
parser.add_argument("-v", "--version", type=str, default='7', choices=['7', '8 WIP'])

args = parser.parse_args()

rhino =  f'C:\Program Files\Rhino {args.version}\System\Rhino.exe'
script = 'OpenSharedModel _Enter _Enter'
args = f'/nosplash /notemplate /runsript="{script}"'

proc.run( [rhino, args] )

sys.exit(0)