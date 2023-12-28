using System;
using System.Net;
using CatalogService.Api.Core.Application.ViewModels;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

#nullable disable

namespace CatalogService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly CatalogContext catalogContext;
        private readonly CatalogSettings catalogSettings;

        public CatalogController(CatalogContext catalogContext, IOptionsSnapshot<CatalogSettings> catalogSettings)
        {
            this.catalogContext = catalogContext;
            this.catalogSettings = catalogSettings.Value;

            catalogContext.ChangeTracker.QueryTrackingBehavior = Microsoft.EntityFrameworkCore.QueryTrackingBehavior.NoTracking;
        }

        [HttpGet]
        [Route("items")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(IEnumerable<CatalogItem>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetItemAsync([FromQuery] int pageSize = 10, [FromQuery] int pageIndex = 0, string ids = null)
        {
            if (!string.IsNullOrEmpty(ids))
            {
                var items = await GetItemsByIdsAsync(ids);

                if (!items.Any())
                {
                    return BadRequest("ids value invalid. Must be comma-seperated list of numbers");
                }
                return Ok(items);
            }

            var totalItems = await catalogContext.CatalogItems.LongCountAsync();

            var itemsOnPage = await catalogContext.CatalogItems.OrderBy(s => s.Name).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            var model = new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);

            return Ok(model);
        }

        private async Task<IList<CatalogItem>> GetItemsByIdsAsync(string ids)
        {
            var numIds = ids.Split(',').Select(id => (OK: int.TryParse(id, out int x), Value: x));

            if (!numIds.All(nid => nid.OK))
            {
                return new List<CatalogItem>();
            }

            var idsToSelect = numIds.Select(s => s.Value);

            var items = await catalogContext.CatalogItems.Where(s => idsToSelect.Contains(s.Id)).ToListAsync();

            items = ChangeUriPlaceholder(items);

            return items;
        }

        [HttpGet]
        [Route("items/{id:int}")]
        [ProducesResponseType(typeof(CatalogItem),(int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CatalogItem>> GetItemId(int id)
        {
            if(id<=0)
            {
                return BadRequest();
            }
            var item = await catalogContext.CatalogItems.SingleOrDefaultAsync(s=>s.Id==id);


            var baseUri = catalogSettings.PicBaseUrl;

            if(item!=null)
            {
                item.PictureUri =baseUri + item.PictureFileName;

                return item;
            }

            return NotFound();
        }

        [HttpGet]
        [Route("items/withname/{name:minlength(1)}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> GetItemWithName(string name,[FromQuery] int pageSize = 10, [FromQuery] int pageIndex=0)
        {
            var totalItems = await catalogContext.CatalogItems.Where(s=>s.Name.StartsWith(name)).LongCountAsync();

            var itemsOnPage = await catalogContext.CatalogItems.Where(s => s.Name.StartsWith(name)).Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex,pageSize,totalItems,itemsOnPage);
        }

        [HttpGet]
        [Route("items/type/{catalogTypeId}/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> GetItemTypeIdBrandId(int catalogTypeId,int? catalogBrandId,int pageSize,int pageIndex)
        {
            var root = (IQueryable<CatalogItem>)catalogContext.CatalogItems;

            root = root.Where(ci=>ci.CatalogTypeId == catalogTypeId);

            if(catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.LongCountAsync();

            var itemsOnPage = await root.Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex,pageSize,totalItems,itemsOnPage);
        }

        [HttpGet]
        [Route("items/type/all/brand/{catalogBrandId:int?}")]
        [ProducesResponseType(typeof(PaginatedItemsViewModel<CatalogItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PaginatedItemsViewModel<CatalogItem>>> GetItemBrandId(int? catalogBrandId, int pageSize, int pageIndex)
        {
            var root = (IQueryable<CatalogItem>)catalogContext.CatalogItems;

            if (catalogBrandId.HasValue)
            {
                root = root.Where(ci => ci.CatalogBrandId == catalogBrandId);
            }

            var totalItems = await root.LongCountAsync();

            var itemsOnPage = await root.Skip(pageSize * pageIndex).Take(pageSize).ToListAsync();

            itemsOnPage = ChangeUriPlaceholder(itemsOnPage);

            return new PaginatedItemsViewModel<CatalogItem>(pageIndex, pageSize, totalItems, itemsOnPage);
        }

        [HttpGet]
        [Route("catalogtypes")]
        [ProducesResponseType(typeof(List<CatalogType>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<CatalogType>>> GetCatalogTypes()
        {
            return await catalogContext.CatalogTypes.ToListAsync();
        }
        [HttpGet]
        [Route("catalogbrands")]
        [ProducesResponseType(typeof(List<CatalogBrand>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<CatalogBrand>>> GetCatalogBrands()
        {
            return await catalogContext.CatalogBrands.ToListAsync();
        }

        [HttpPut]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> UpdateProduct([FromBody] CatalogItem productToUpdate)
        {
            var catalogItem = await catalogContext.CatalogItems.SingleOrDefaultAsync(s => s.Id == productToUpdate.Id);

            if(catalogItem==null)
            {
                return NotFound(new {Message=$"Item with id {productToUpdate.Id} not found!"});
            }

            var oldPrice = catalogItem.Price;

            var raiseProductPriceChangedEvent = oldPrice != productToUpdate.Price;

            catalogItem = productToUpdate;

            catalogContext.CatalogItems.Update(catalogItem);

            await catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemId),new {id=productToUpdate.Id},null);
        }

        [HttpPost]
        [Route("items")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<ActionResult> CreateProduct([FromBody] CatalogItem product)
        {
            var item = new CatalogItem
            {
                CatalogBrandId=product.CatalogBrandId,
                CatalogTypeId=product.CatalogTypeId,
                Description=product.Description,
                Name=product.Name,
                PictureFileName=product.PictureFileName,
                Price=product.Price
            };

            catalogContext.CatalogItems.Add(item);
            await catalogContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItemId), new { id = item.Id }, null);
        }

        [HttpDelete]
        [Route("items/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product =await catalogContext.CatalogItems.SingleOrDefaultAsync(s => s.Id == id);

            if(product ==null)
            {
                return NotFound();
            }

            catalogContext.CatalogItems.Remove(product);

            await catalogContext.SaveChangesAsync();

            return NoContent();
        }



        private List<CatalogItem> ChangeUriPlaceholder(List<CatalogItem> items)
        {
            var baseUri = catalogSettings.PicBaseUrl;

            foreach (var item in items)
            {
                if (item != null)
                {
                    item.PictureUri = baseUri + item.PictureFileName;
                }
            }
            return items;
        }
    }
}

