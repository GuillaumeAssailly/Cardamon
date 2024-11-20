using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PokéService.Data;
using Common.PokéEntities;

namespace PokéService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokéController : ControllerBase
    {
        private readonly PokéServiceContext _context;

        public PokéController(PokéServiceContext context)
        {
            _context = context;
        }

        // Collection: api/Poké/collection

        // GET: api/Poké/collection
        [HttpGet("collection")]
        public ActionResult<string[]?> GetAllCollections() // get all collections
        {
            var c = _context.CardsCollection.Select(p => p.Id);

            return Ok(c);
        }

        // DELETE: api/Poké/collection
        [HttpDelete("collection")]
        public async Task<ActionResult> DeleteCollections() // delete all collections
        {
            foreach (var c in _context.CardsCollection) _context.CardsCollection.Remove(c);
            await _context.SaveChangesAsync();
            return Ok();
		}


        // GET: api/Poké/collection/public/5
        [HttpGet("collection/public/{id}")]
        public async Task<ActionResult<bool>> IsPublic(int id) // is collection public
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is null) return NotFound("collection not found");

            return Ok(c.IsPublic);
        }


        // POST: api/Poké/collection/5/True
        [HttpPost("collection/public/{id}/{isPublic}")]
        public async Task<ActionResult> SetProtection(int id, bool isPublic) // set collection protection
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is null) return NotFound("collection not found");

            c.IsPublic = isPublic;

            _context.Entry(c).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok();
        }


        // GET: api/Poké/collection/5
        [HttpGet("collection/{id}")]
		public async Task<ActionResult<string[]?>> GetCollection(int id) // get all cards ids
		{
			var c = await _context.CardsCollection.FindAsync(id);

			if (c is null) return NotFound("collection not found");

			return Ok(c.CardsArray);
		}


		// GET: api/Poké/collection/5/contains/base1-1+base1-2
		[HttpGet("collection/{id}/contains/{cid}")]
		public async Task<ActionResult<IEnumerable<bool>>> CheckCollection(int id, string cid) // check if collection contains cards
		{
			var c = await _context.CardsCollection.FindAsync(id);

			if (c is null) return NotFound("collection not found");

            var b = new List<bool>();
            var s = cid.Split('+');

            var ca = c.CardsArray;

            foreach (var i in s) b.Add(ca != null && ca.Contains(i));
            
			return Ok(b);
		}


		// GET: api/Poké/collection/5/card?id=&name=&set=&category=&lid=&type1=&type2=&hp=&illustrator=&limit=&offset=
		[HttpGet("collection/{uid}/card")]
        public async Task<ActionResult<IEnumerable<Pokémon>>> GetPokémonCollection( // get cards data
            int uid, // user to get from
            int? id = null, // user id to attach the "possessed" property, uid if None
            string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            var c = await _context.CardsCollection.FindAsync(uid);

            if (c is null) return NotFound("collection not found");

            var p = new List<Pokémon>();

            string[]? cd = c.CardsArray;

            if (cd is not null) foreach (var cid in cd)
            {
                var pk = await _context.Pokémon.FindAsync(cid);
                if (pk is not null) p.Add(pk);
            }

            var pkmns = Search(p.AsQueryable(), name, set, category, lid, type1, type2, illustrator, hp, limit, offset);

            if (pkmns is null) return NotFound();

            if (id is not null)
            {
                var c2 = await _context.CardsCollection.FindAsync(id);

                if (c2 is not null)
                {
                    var ca = c2.CardsArray;
                    if (ca is not null) foreach (var pk in pkmns) pk.Possessed = ca.Contains(pk.Id);
                }
            } 
            else foreach (var pk in pkmns) pk.Possessed = true;

            return Ok(pkmns);
        }


        // POST: api/Poké/collection/5
        [HttpPost("collection/{id}")]
        public async Task<ActionResult> CreateCollection(int id) // create collection
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is not null) return Ok(); // Already exists

            await _context.AddAsync(new CardsCollection { Id = id, IsPublic = false });
            await _context.SaveChangesAsync();
            return Created();
        }


        // DELETE: api/Poké/collection/5
        [HttpDelete("collection/{id}")]
        public async Task<ActionResult> DeleteCollection(int id) // delete collection
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is null) return NotFound("collection not found");

            _context.CardsCollection.Remove(c);
            await _context.SaveChangesAsync();

            return Ok();
        }


        // POST: api/Poké/collection/5/card/1
        [HttpPost("collection/{id}/card/{cid}")]
        public async Task<ActionResult> AddCard(int id, string cid) // add card to collection
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is null) return NotFound("collection not found");

            var p = await _context.Pokémon.FindAsync(cid);

            if (p is null) return NotFound("card not found");

            if (c.AddCard(cid))
            {
                await _context.SaveChangesAsync();
                return Ok();
            }

            return BadRequest("card already in the collection");
        }

        // DELETE: api/Poké/collection/5/card/1
        [HttpDelete("collection/{id}/card/{cid}")]
        public async Task<ActionResult> DeleteCard(int id, string cid) // delete card from collection
        {
            var c = await _context.CardsCollection.FindAsync(id);

            if (c is null) return NotFound("collection not found");

            if (c.RemoveCard(cid))
            {
                await _context.SaveChangesAsync();
                return Ok();
            }

            return NotFound("card not in the collection");
        }



        // Pokémon: api/Poké/card

        // GET: api/Poké/card/5?id=
        [HttpGet("card/{cid}")]
        public async Task<ActionResult<Pokémon>> GetCard(string cid, int? id = null)  // user id to attach the "possessed" property, ignored if invalid
		{
            var p = await _context.Pokémon.FindAsync(cid);

            if (p is null) return NotFound();

            if (id is not null)
            {
                var c = await _context.CardsCollection.FindAsync(id);

                if (c is not null)
                {
					var ca = c.CardsArray;

					if (ca is not null) p.Possessed = ca.Contains(p.Id);
				}
            }

			return Ok(p);
        }


        // GET: api/Poké/search?id=&name=&set=&category=&lid=&type1=&type2=&hp=&illustrator=&limit=&offset=
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Pokémon>>> SearchCard(
            int? id = null, // user id to attach the "possessed" property, ignored if invalid
			string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            var pkmns = Search(_context.Pokémon.AsQueryable(),
                name, set, category, lid, type1, type2, illustrator, hp, limit, offset);

            if (pkmns is null) return NotFound();

            if (id is not null)
            {
			    var c = await _context.CardsCollection.FindAsync(id);

                if (c is not null)
				{
					var ca = c.CardsArray;

					if (ca is not null) foreach (var p in pkmns)
                    {
                        p.Possessed = ca.Contains(p.Id);
                    }
                }
            }

			return Ok(pkmns);
        }


        private IEnumerable<Pokémon>? Search(
            IQueryable<Pokémon>? pkmns,
            string? name = null,
            string? set = null,
            Categories? category = null,
            string? lid = null,
            string? type1 = null,
            string? type2 = null,
            string? illustrator = null,
            int? hp = null,
            int limit = 50,
            int offset = 0)
        {
            if (pkmns is null) return null;
            if (name is not null) pkmns = pkmns.Where(p => p.Name.ToLower().StartsWith(name.ToLower()) ||    // check name start: "Pika" -> "Pikachu"
                                                       p.Name.ToLower().Contains(' ' + name.ToLower())); // check subwords starts: "Pika" -> "Trainer's Pikachu"
            if (set is not null) pkmns = pkmns.Where(p => p.Set == set);
            if (category is not null) pkmns = pkmns.Where(p => p.Category == category);
            if (lid is not null) pkmns = pkmns.Where(p => p.LocalId == lid);
            if (type1 is not null) pkmns = pkmns.Where(p => p.Types != null ? p.Types.Contains(type1) : false);
            if (type2 is not null) pkmns = pkmns.Where(p => p.Types != null ? p.Types.Contains(type2) : false);
            if (hp is not null) pkmns = pkmns.Where(p => p.Hp == hp);
            if (illustrator is not null) pkmns = pkmns.Where(p => p.Illustrator.Contains(illustrator));
            pkmns = pkmns.OrderBy(p => p.Types).OrderBy(p => p.Name);
            return pkmns.Skip(offset).Take(limit);
        }
    }
}
