import clr
import os
import sys

bin_dir = os.path.join(os.path.dirname(os.path.realpath(__file__)), "..", "bin")

if __name__ == "__main__":
  sys.exit(0)
#  clr.AddReference(os.path.join(bin_dir, "module.dll"))
#  from SpecificNamespace import Validator
#  result = Validator.ValidatePassword("123aioawdhaXAWDFIAWHxAIDUA#.W")
#  print(result)
