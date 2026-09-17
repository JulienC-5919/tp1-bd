import random
import string

def genererNumeroSerie():
    str = "".join(random.choices(string.ascii_uppercase, k=2)) 
    return str + "".join(random.choices(string.digits, k=10))

print(genererNumeroSerie())