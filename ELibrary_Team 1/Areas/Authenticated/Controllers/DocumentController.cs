﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ELibrary.Data;
using ELibrary_Team_1.Models;
using ELibrary_Team1.DataAccess.Data.Repository.IRepository;
using ELibrary_Team_1.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace ELibrary_Team_1.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Route("Authenticated/[controller]/[action]")]
    public class DocumentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;


        public DocumentController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Documents
        public IActionResult Index()
        {
            return View(_unitOfWork.Document.GetAll());
        }

    // Create Document
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            IEnumerable<Category> categoryList = await _unitOfWork.Category.GetAllAsync();
            DocumentViewModel documentVM = new DocumentViewModel()
            {
                // Pass Category list from db.Category to SelectListItem
                CategoryList = categoryList.Select(l => new SelectListItem
                {
                    Text = l.Title,
                    Value = l.Id.ToString()
                })
            };
            return View(documentVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DocumentViewModel documentVM)
        {
            if (ModelState.IsValid)
            {
                if(documentVM.ImageFile != null)
                {
                    // UploadedFile method: Check if imange exists and copy Image file to webRoot
                    documentVM.Document.Image = UploadedFile(documentVM);
                }
               
                _unitOfWork.Document.Add(documentVM.Document);
                _unitOfWork.SaveChange();
                if(documentVM.SelectedCategory != null)
                {
                    foreach (var item in documentVM.SelectedCategory)
                    {
                        // Add selected category for document to db.DocumentCategory
                        _unitOfWork.DocumentCategory.Add(new DocumentCategory { DocumentId = documentVM.Document.Id, CategoryId = item });
                    }
                    return RedirectToAction(nameof(Index));
                }
                await _unitOfWork.SaveChangeAsync();
            }
             
            
             return View(documentVM);

        }
    // Edit Document
        public IActionResult Edit(int id)
        {
            var model = new DocumentViewModel();
            var document = _unitOfWork.Document.GetById(id);
            var documentCategory = _unitOfWork.DocumentCategory.GetAll().Where(x => x.DocumentId == id);
            IEnumerable <Category> categoryList = _unitOfWork.Category.GetAll().ToList();
            model = new DocumentViewModel()
            {
                Document = document,
                SelectedCategory = documentCategory.Select(x => x.CategoryId).ToList(),
                CategoryList = categoryList.Select(l => new SelectListItem
                {
                    Text = l.Title,
                    Value = l.Id.ToString()
                })
            };
            if(model.Document.Image != null)
            {
                string wwwrootPath = _hostEnvironment.WebRootPath;
                string path = Path.Combine(wwwrootPath + "/images/" + document.Image);
                using (var stream = System.IO.File.OpenRead(path))
                {
                    model.ImageFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                }
            }
                
            return View(model);

        }
        [HttpPost]
        public IActionResult Edit(DocumentViewModel documentVM)
        {
            if(ModelState.IsValid)
            {
                if (documentVM.ImageFile != null)
                {
                    documentVM.Document.Image = UploadedFile(documentVM);
                }
                _unitOfWork.Document.Update(documentVM.Document);
                if (documentVM.SelectedCategory != null)
                {
                    var oldDocumentCategories = _unitOfWork.DocumentCategory.GetAll().Where(x => x.DocumentId == documentVM.Document.Id);
                    _unitOfWork.DocumentCategory.RemoveRange(oldDocumentCategories);
                    foreach (var item in documentVM.SelectedCategory)
                    {
                        _unitOfWork.DocumentCategory.Add(new DocumentCategory { DocumentId = documentVM.Document.Id, CategoryId = item });
                    }
                }
                _unitOfWork.SaveChange();
                return RedirectToAction(nameof(Index));
            }
            _unitOfWork.SaveChange();
            
            return View(documentVM);
        }



        // Delete Method
        public IActionResult Delete(int id)
        {
            var document = _unitOfWork.Document.GetById(id);

            return View(document);
        }
        [HttpPost]
        public IActionResult Delete(Document document)
        {
            if (document.Image != null) // Delete old image
            {
                string wwwrootPath = _hostEnvironment.WebRootPath;
                string imagePath = Path.Combine(wwwrootPath + "/images/" + document.Image);
                System.IO.File.Delete(imagePath);
            }
            _unitOfWork.Document.Remove(document);
            
           
            _unitOfWork.SaveChange();
            return RedirectToAction(nameof(Index));
        }















        private string UploadedFile(DocumentViewModel model)
        {
            string wwwrootPath = _hostEnvironment.WebRootPath;
            string newFileName = null;

            if(model.Document.Image != null) // Delete old image
            {
                string oldFilePath = Path.Combine(wwwrootPath + "/images/" + model.Document.Image);
                System.IO.File.Delete(oldFilePath);
            }    

            newFileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
            string extension = Path.GetExtension(model.ImageFile.FileName);
            newFileName = newFileName + Guid.NewGuid().ToString() + extension;
            string newFilePath = Path.Combine(wwwrootPath + "/images/" + newFileName);
            
            using (var fileStream = new FileStream(newFilePath, FileMode.Create))
            {
                model.ImageFile.CopyTo(fileStream);
            }
         
            return newFileName;
        }
        
    }

}
