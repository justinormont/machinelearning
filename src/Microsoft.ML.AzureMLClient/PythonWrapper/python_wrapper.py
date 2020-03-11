import argparse
import subprocess
import sys

# This is a wrapper script that can be used by the ScriptRunConfig to launch python code that is baked in
# the Docker image.  It can be used by either the Python or C# SDK to orchestrate code that is not managed
# by Azure ML.

# For example, if the python interpreter is located at /opt/miniconda/envs/azureml/bin/python in the Docker
# container, and the python script is called /azureml/train.py,

# runConfiguration.ScriptFile = new FileInfo(@"/Users/srmorin/train/python_wrapper.py");
# runConfiguration.Arguments =
#    new List<string> {"--spawnprocess", "/opt/miniconda/envs/azureml/bin/python", "/azureml/train.py"}

parser = argparse.ArgumentParser(description='Execute code baked into a Docker Container from AzureML ScriptRunConfig')
parser.add_argument('--spawnprocess', dest='spawnprocess', action='store')
args, unknown_args = parser.parse_known_args()

p = subprocess.Popen([str(args.spawnprocess)] + unknown_args,
                     shell=False, stdout=subprocess.PIPE, stderr=subprocess.STDOUT)

# Read all the lines that are printed by the subprocess to report out to the
for line in p.stdout.readlines():
    print(line.decode('UTF-8').strip())
retval = p.wait()

# return the subprocess error code to the calling job so that it is reported to AzureML
sys.exit(retval)
