"""
Experimental script to dump all cards from TCGdex API
- try to fix some missing data by merging Fr and En data
- dumping process is very slow (+1h for ~15000 cards)
- generate pkmn.all.json
"""

import requests as r, json

base = "https://api.tcgdex.net/v2/fr/{}"
baseEn = "https://api.tcgdex.net/v2/en/{}"

g = r.get

CAT = {
    'Énergie': 1,
    'Pokémon': 0,
    'Dresseur': 2
    }

def p(n):
    if n % 500 == 0: print(n)
    return True

try:
    
    cards = json.load(open("data.f.json", "r", encoding="utf-8"))
    
except:

    try:
        cards = json.load(open("data.json", "r", encoding="utf-8"))

    except:
    
        print("Dl cards... ", end = "")
        cards = g(base.format("cards")).json()
        print(len(cards))

        print("Dl cards data...")
        cards = [g(base.format(f"cards/{i['id']}")).json() for n, i in enumerate(cards) if p(n)]
        json.dump(cards, open("data.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)


    print("Format data...")

    DS = {}

    for n, i in enumerate(cards):
        p(n)
        if not i: raise
        I = i.copy()
        
        sid = i['set']['id']
        Sid = sid
        while Sid[-1].isdigit() or Sid[-1] == ".": Sid = Sid[:-1]
        if sid == Sid:
            if Sid.endswith("p"): Sid = Sid[:-1]
            else: print("=", Sid, sid)
        sid1 = Sid + "1"

        if "image" in i:
            i["image"] += "/high.png"
        else:
            i["image"] = None  # f"https://assets.tcgdex.net/en/{Sid}/{sid}/{i['localId']}/high.png"
            
        i["total"] = i["set"]["cardCount"]["official"]
        
        u = i["set"].get("logo", None)
        if u is None:
            i["setImage"] = None
            # u = f"https://assets.tcgdex.net/en/{Sid}/{sid}/logo"
        else:
            i["setImage"] = u+".png"
            
            
        i["set"] = i["set"].get("name")
        i["cid"] = i["id"]
        i["id"] = n
        del i["variants"]
        del i["legal"]

        if i["set"] is None or i["setImage"] is None:
            D = DS.get(sid)
            if D is None:
                if sid != sid1:
                    N = DS.setdefault(sid, g(baseEn.format(f"sets/{sid1}")).json() | g(base.format(f"sets/{sid1}")).json())
                else:
                    N = {}
                D = DS.setdefault(sid, N | g(baseEn.format(f"sets/{sid}")).json() | g(base.format(f"sets/{sid}")).json())
                
            if i["set"] is None:
                if sid.lstrip(Sid) != "1":
                    c = D["serie"]["name"] + " "
                else: c = ""
                i["set"] = c + D["name"]
                
            if i["setImage"] is None:
                i["setImage"] = D.get("logo")
                if i["setImage"] is None:
                    print("missing set", sid, n)
                    #D["logo"] = i["setImage"] = f'https://assets.tcgdex.net/en/{Sid}/{Sid}1/logo'
                else: i["setImage"] += '.png'

        if i["image"] is None:
            V = g(baseEn.format(f"cards/{i['cid']}")).json()
            i["image"] = V.get("image")
            if i["image"] is None:
                print("missing img", sid, i["set"], n)
            else:
                i["image"] += "/high.png"
    cards = [i for i in cards if i["image"]]
    
    for i in cards:
        if i["setImage"] == "https://assets.tcgdex.net/fr/swsh/swshp/logo.png":
            print("fix set img", i["cid"])
            i["setImage"] = "https://assets.tcgdex.net/fr/swsh/swsh1/logo.png"
        if i["setImage"] is None:
            if not i["cid"].startswith("sma"): raise ValueError
            i["setImage"] = "https://assets.tcgdex.net/en/sm/sm1/logo.png"

        i["category"] = CAT[i["category"]]

        if "illustrator" not in i: i["illustrator"] = "Inconnue"
        if "dexId" in i: del i["dexId"]

        if "attacks" in i:
            for u in i["attacks"]:
                if "damage" in u: u["damage"] = str(u["damage"])
        
        i["id"] = i["cid"]
        del i["cid"]
            
    json.dump(cards, open("data.f.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
                

x = [{"A": None, "B": None}, {"A": None, "B": None}, {"A": None, "B": None}]

for i in cards:
    c=x[i["category"]]
    if c["A"] is None: c["A"] = set(i)
    else: c["A"] &= set(i)
    if c["B"] is None: c["B"] = set(i)
    else: c["B"] |= set(i)
    for u in i:
        if u not in c:
            c[u] = set()
        try:
            c[u].add(tuple(i[u]) if isinstance(i[u], list) else i[u])
        except:
            if isinstance(i[u], dict):
                c[u] = i[u] if isinstance(c[u], set) else (c[u] | i[u])
            else:
                w = {}
                for v in i[u]: w |= v
                c[u] = w if isinstance(c[u], set) else (c[u] | w)


class FrozenDict(dict):
    
    def freeze(self):
        for i in self:
            if isinstance(self[i], list):
                self[i] = tuple(self[i])
            elif isinstance(self[i], dict) and not isinstance(self[i], FrozenDict):
                self[i] = FrozenDict(self[i])
        self["id"] = hash(self)
        return self
                
    def __hash__(self):
        return sum(map(hash, self.values())) - hash(self.get("id", 0))

    
attacks = set()
weaknesses = set()
resistances = set()
abilities = set()
items = set()

ori = {}

for i in cards:
    t = i.get("types", [])
    if t: i["types"] = "\n".join(t)
        
    for u in i.get("attacks", []):
        t = u.get("cost", [])
        if t: u["cost"] = "\n".join(t)

json.dump(cards, open("pkmn.all.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
"""
for n, i in enumerate(cards):
    ori[n] = {}
    if "attacks" in i:
        ori[n] |= {"attacks": (b := [])}
        t = set()
        for u in i["attacks"]:
            attacks.add(v := FrozenDict(u).freeze())
            t.add(v["id"])
            b.append(v)
        i["attacks"] = tuple(t)
        
    if "weaknesses" in i:
        ori[n] |= {"weaknesses": (b := [])}
        t = set()
        for u in i["weaknesses"]:
            weaknesses.add(v := FrozenDict(u).freeze())
            t.add(v["id"])
            b.append(v)
        i["weaknesses"] = tuple(t)
        
    if "resistances" in i:
        ori[n] |= {"resistances": (b := [])}
        t = set()
        for u in i["resistances"]:
            resistances.add(v := FrozenDict(u).freeze())
            t.add(v["id"])
            b.append(v)
        i["resistances"] = tuple(t)
        
    if "abilities" in i:
        ori[n] |= {"abilities": (b := [])}
        t = set()
        for u in i["abilities"]:
            abilities.add(v := FrozenDict(u).freeze())
            t.add(v["id"])
            b.append(v)
        i["abilities"] = tuple(t)
        
    if "item" in i:
        abilities.add(v := FrozenDict(i["item"]).freeze())
        i["item"] = v["id"]
        ori[n] |= {"item": v}

    if not ori[n]: del ori[n]


for i in cards:
    if "attacks" in i:
        i["attacks"] = tuple({"attackId": u, "PokémonId": i["id"]} for u in i["attacks"])
    if "weaknesses" in i:
        i["weaknesses"] = tuple({"weaknessId": u, "PokémonId": i["id"]} for u in i["weaknesses"])
    if "resistances" in i:
        i["resistances"] = tuple({"resistanceId": u, "PokémonId": i["id"]} for u in i["resistances"])
    if "abilities" in i:
        i["abilities"] = tuple({"abilityId": u, "PokémonId": i["id"]} for u in i["abilities"])
    if "item" in i:
        i["item"] = {"itemId": i["item"], "PokémonId": i["id"]}
           
json.dump(cards, open("pkmn.card.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
json.dump(tuple(attacks), open("pkmn.att.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
json.dump(tuple(weaknesses), open("pkmn.weak.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
json.dump(tuple(resistances), open("pkmn.res.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
json.dump(tuple(abilities), open("pkmn.abi.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
json.dump(tuple(items), open("pkmn.item.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)
    
"""
# rebuild original cards
"""
for i, u in ori.items():
    cards[i] |= u

json.dump(cards, open("pkmn.full.json", "w", encoding="utf-8"), ensure_ascii=False, indent=4)"""
    
