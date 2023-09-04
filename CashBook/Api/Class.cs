namespace CashBook.Api
{
        [Route("api/practiceProblems")]
        [EnableCors()]
        public class PracticeProblemsController : BaseController
        {
            private readonly IClassScheduleService _classScheduleService;
            private readonly IPracticeProblemsFileUpload _practiceProblemsFileUpload;
            private readonly IPracticeProblemsMCQ _practiceProblemsMCQ;
            private readonly IPracticeProblemsMcqAnswers _practiceProblemsMcqAnswers;
            private readonly ResourceUriSettings uriSettings;
            private readonly IUserService<AuthorizedUser> userService;
            private readonly IMapper _mapper;
            private readonly ICommonRepository _commonRepository;
            AppDbContext _db;
            private readonly IConfiguration _config;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly IWebHostEnvironment _webHostEnvironment;



            public PracticeProblemsController(IClassScheduleService classScheduleService,
                IPracticeProblemsFileUpload practiceProblemsFileUpload, IPracticeProblemsMCQ practiceProblemsMCQ,
                IPracticeProblemsMcqAnswers practiceProblemsMcqAnswers, IMapper mapper,
                AppDbContext db,
                ResourceUriSettings uriSettings, IConfiguration config, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment,
            IUserService<AuthorizedUser> userService) : base(uriSettings, userService)
            {
                _classScheduleService = classScheduleService;
                _practiceProblemsFileUpload = practiceProblemsFileUpload;
                _practiceProblemsMCQ = practiceProblemsMCQ;
                _practiceProblemsMcqAnswers = practiceProblemsMcqAnswers;
                _mapper = mapper;
                _db = db;
                _config = config;
                _httpContextAccessor = httpContextAccessor;
                _webHostEnvironment = webHostEnvironment;


            }

            #region fileupload

            [HttpGet]
            [Route("fileUploadList")]
            public async Task<IActionResult> GetAllPracticeProblemsFileUpload()
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var User = CurrentUser;
                if (User.IsInRole(UserRoleNames.RoleName(RoleList.SubjectTeacher)) || User.IsInRole(UserRoleNames.RoleName(RoleList.SuperAdmin)) || User.IsInRole(UserRoleNames.RoleName(RoleList.ClassTeacher)) || User.IsInRole(UserRoleNames.RoleName(RoleList.Principal)))
                {
                    var fileList = new List<PracticeProblemsFileUpload>();
                    fileList = _practiceProblemsFileUpload.PracticeProblemsFileUploadList();
                    var practiceproblemfileuploadDto = new List<PracticeProblemsFileUploadDto>();


                    _mapper.Map(fileList, practiceproblemfileuploadDto);


                    if (practiceproblemfileuploadDto == null)
                    {
                        return NotFound();
                    }

                    else
                    {
                        return Ok(practiceproblemfileuploadDto);
                    }
                }

                else
                {
                    return BadRequest("Not Authorized to perform this action!..");
                }
            }
            [HttpGet]
            [Route("fileUpload/{practiceProblemId}")]
            public async Task<IActionResult> GetpracticeProblemFileUploadById(int? practiceProblemId)
            {
                if (ModelState.IsValid)
                {

                    if (practiceProblemId == null)
                    {
                        return BadRequest();
                    }


                    List<PracticeProblemsFileUpload> practiceProblemsFiles = _db.PracticeProblemsFileUploadMain.Include(s => s.FileDetails).Include(c => c.Classes).Include(s => s.Subjects).Where(o => o.Id == practiceProblemId).ToList();
                    if (practiceProblemsFiles == null)
                    {
                        return NotFound();
                    }
                    //
                    else
                        return Ok(practiceProblemsFiles);

                }
                else
                {
                    return BadRequest();
                }
            }
            [HttpPost]
            [Route("fileUpload")]
            public async Task<IActionResult> PostPracticeProblems(long ClassId, long SubjectId, long TopicId, string Name, DateTime CreatedDate)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                Dictionary<string, string> ZipValues = new Dictionary<string, string>();
                List<PracticeProblemsFileUpload> practiceProblemFUList = await _practiceProblemsFileUpload.GetAllAsync();
                PracticeProblemsFileUpload practiceProblemsFileUpload = new PracticeProblemsFileUpload();
                List<PracticeProblemsFileUploadDetail> fileUploadList = new List<PracticeProblemsFileUploadDetail>();
                var practiceproblmfileuploaddto = new PracticeProblemsFileUploadDto();
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {
                    /*if (practiceProblemFUList.Any(x => x.Name.ToLower().Trim() == Name.ToLower().Trim()))
                        return BadRequest($"Practice Problem {Name} Already Exists");*/

                    practiceProblemsFileUpload.Name = Name;
                    practiceProblemsFileUpload.SubjectId = SubjectId;
                    practiceProblemsFileUpload.ClassId = ClassId;
                    practiceProblemsFileUpload.TopicId = TopicId;
                    practiceProblemsFileUpload.CreatedDate = CreatedDate;
                    practiceProblemsFileUpload.CreatedBy = CurrentUser.Id;



                    try
                    {
                        var httpPostedFile = _httpContextAccessor.HttpContext.Request.Form.Files.Count > 0 ? _httpContextAccessor.HttpContext.Request.Form.Files[0] : null;

                        var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files;

                        if (httpPostedFile != null && httpPostedFile.Length > 0)
                        {

                            for (int i = 0; i < postedFile.Count; i++)

                            {

                                var fileName = Path.GetFileName(postedFile[i].FileName);

                                string fileExtension = System.IO.Path.GetExtension(postedFile[i].FileName);
                                if (IsValidExtension(fileExtension))
                                {
                                    PracticeProblemsFileUploadDetail fileDetail = new PracticeProblemsFileUploadDetail()
                                    {

                                        FileName = fileName,
                                        FileExtension = fileExtension,
                                        CreatedOn = practiceProblemsFileUpload.CreatedOn,
                                        IsActive = true,
                                        RefId = practiceProblemsFileUpload.RefId,
                                        PracticeProblemsFUMainId = practiceProblemsFileUpload.Id


                                    };
                                    fileUploadList.Add(fileDetail);
                                }
                                else
                                {
                                    return BadRequest(fileName + "not valid");
                                }
                                string webRootPath = _webHostEnvironment.WebRootPath;
                                string contentRootPath = _webHostEnvironment.ContentRootPath;

                                var filepath = Path.Combine(webRootPath, "Uploads", "Templates", fileName);
                                var path = Path.Combine(webRootPath, "Uploads");
                                // Construct file save path  

                                //var path = System.Web.Hosting.HostingEnvironment.MapPath(_config["AppSettings:fileUploadFolder"]);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                path = Path.Combine(webRootPath, "Uploads", "Templates");
                                //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(_config["AppSettings:fileUploadFolder"]), fileName);

                                //if (File.Exists(fileSavePath))
                                //{
                                //    File.Delete(fileSavePath);
                                //}
                                //// Save the uploaded file  

                                //postedFile[i].SaveAs(fileSavePath);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }

                            }
                        }
                        practiceProblemsFileUpload.FileDetails = fileUploadList;
                        _db.PracticeProblemsFileUploadMain.Add(practiceProblemsFileUpload);
                        _db.SaveChanges();


                        _mapper.Map(practiceProblemsFileUpload, practiceproblmfileuploaddto);

                    }
                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message.ToString());
                    }


                }
                return Ok(practiceproblmfileuploaddto);
            }




            [HttpPut]
            [Route("updateFile")]
            public async Task<IActionResult> PutPracticeProblemsFileUpload(long Id, long ClassId, long SubjectId, long TopicId, string Name, DateTime CreatedDate)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);

                PracticeProblemsFileUpload practiceProblemsFileUpload = new PracticeProblemsFileUpload();
                List<PracticeProblemsFileUploadDetail> fileUploadList = new List<PracticeProblemsFileUploadDetail>();

                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {

                    practiceProblemsFileUpload = _db.PracticeProblemsFileUploadMain.Include(s => s.FileDetails).Where(o => o.Id == Id).SingleOrDefault();

                    practiceProblemsFileUpload.Name = Name;
                    practiceProblemsFileUpload.SubjectId = SubjectId;
                    practiceProblemsFileUpload.ClassId = ClassId;
                    practiceProblemsFileUpload.TopicId = TopicId;
                    practiceProblemsFileUpload.CreatedDate = CreatedDate;
                    practiceProblemsFileUpload.CreatedBy = CurrentUser.Id;

                    try
                    {
                        var httpPostedFile = _httpContextAccessor.HttpContext.Request.Form.Files.Count > 0 ? _httpContextAccessor.HttpContext.Request.Form.Files[0] : null;

                        var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files;

                        if (httpPostedFile != null && httpPostedFile.Length > 0)
                        {

                            for (int i = 0; i < postedFile.Count; i++)

                            {

                                var fileName = Path.GetFileName(postedFile[i].FileName);
                                string fileExtension = System.IO.Path.GetExtension(postedFile[i].FileName);
                                PracticeProblemsFileUploadDetail fileDetail = new PracticeProblemsFileUploadDetail()
                                {
                                    FileName = fileName,
                                    FileExtension = fileExtension,
                                    CreatedOn = practiceProblemsFileUpload.CreatedOn,
                                    IsActive = true,
                                    RefId = practiceProblemsFileUpload.RefId,
                                    PracticeProblemsFUMainId = practiceProblemsFileUpload.Id
                                };

                                // fileUploadList.Add(fileDetail);
                                //var path = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]);
                                //if (!Directory.Exists(path))
                                //{
                                //    Directory.CreateDirectory(path);
                                //}

                                //// Construct file save path  
                                //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), fileName);

                                //if (File.Exists(fileSavePath))
                                //{
                                //    File.Delete(fileSavePath);
                                //}
                                ////// Save the uploaded file  

                                //postedFile[i].SaveAs(fileSavePath);
                                //_context.Entry(fileDetail).State = EntityState.Added;
                                string webRootPath = _webHostEnvironment.WebRootPath;
                                string contentRootPath = _webHostEnvironment.ContentRootPath;

                                var filepath = Path.Combine(webRootPath, "Uploads", "Templates", fileName);

                                var path = Path.Combine(webRootPath, "Uploads");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                path = Path.Combine(webRootPath, "Uploads", "Templates");
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }


                                _db.SaveChanges();
                                _db.Entry(fileDetail).State = Microsoft.EntityFrameworkCore.EntityState.Added;


                            }
                        }
                    }

                    catch (Exception ex)
                    {
                        return BadRequest(ex.Message.ToString());
                    }

                    _db.Entry(practiceProblemsFileUpload).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _db.SaveChanges();
                    var practiceProblemsFilesuploaddto = new PracticeProblemsFileUploadDto();
                    _mapper.Map(practiceProblemsFileUpload, practiceProblemsFilesuploaddto);
                    return Ok(practiceProblemsFilesuploaddto);
                }

                else
                {
                    return BadRequest();
                }

            }

            [HttpDelete]
            [Route("deleteFile")]
            public async Task<IActionResult> DeletePracticeProblemFileUpload(long Id)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {

                    PracticeProblemsFileUpload practiceProblemsFiles = _db.PracticeProblemsFileUploadMain.Include(s => s.FileDetails).Where(o => o.Id == Id).SingleOrDefault();
                    if (practiceProblemsFiles == null)
                    {
                        return BadRequest("Not Found");
                    }

                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    foreach (var item in practiceProblemsFiles.FileDetails)
                    {
                        //String path = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]), item.FileName);
                        var path = Path.Combine(webRootPath, "Uploads", "Templates", item.FileName);
                        if (Directory.Exists(path))
                        {
                            Directory.Delete(path);
                        }
                    }
                    //
                    _db.PracticeProblemsFileUploadMain.Remove(practiceProblemsFiles);
                    _db.SaveChanges();

                    return Ok("Practice Problem & Files deleted successfully");
                }

                else
                {
                    return BadRequest("Your Not Authorized to Delete Practice Problem");
                }
            }

            [HttpGet]
            [Route("FileUploadAnswersStudentList")]
            public async Task<IActionResult> PracticeProblemViewStudentAnswers(long fileUploadId, long classId, long subjectId)
            {


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {

                    var answersListDtos = (from user in _db.Users
                                           join pps in _db.PracticeProblemsStudentFileUploads on user.Id equals pps.StudentId
                                           where pps.FileUploadMainId == fileUploadId && pps.ClassId == classId && pps.SubjectId == subjectId
                                           select new
                                           {
                                               Name = user.FirstName + "  " + user.LastName,
                                               className = pps.Classes.Name,
                                               pps.ClassId,
                                               pps.SubjectId,
                                               SubjectName = pps.Subjects.Name,
                                               pps.StudentId,
                                               pps.FileUploadMainId,
                                               pps.CreatedDate
                                           })
                                                   .GroupBy(n => n.StudentId).Select(grp => grp.FirstOrDefault()).ToList();

                    if (answersListDtos.Any() == false)
                    {
                        return BadRequest("Not Found");
                    }
                    else
                    {
                        return Ok(answersListDtos);
                    }


                }

                else
                {
                    return BadRequest("Failed to View Student Answers");
                }
            }


            [HttpGet]
            [Route("FileUploadDownloadStudentAnswersZipFileById")]
            public async Task<IActionResult> FileUploadDownloadStudentAnswersZipFileById(string studentId, long fileUploadId, long classId, long subjectId)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {



                    List<PracticeProblemsStudentFileUpload> practiceProblemsStudentFiles = _db.PracticeProblemsStudentFileUploads.Include(f => f.Students).Where(o => o.StudentId == studentId && o.FileUploadMainId == fileUploadId && o.ClassId == classId && o.SubjectId == subjectId).ToList();
                    if (practiceProblemsStudentFiles.Any() == false)
                    {
                        return BadRequest("Not Found");
                    }
                    var studentDetails = _db.Users.Select(s => new { s.UserName, s.Id }).Where(u => u.Id == studentId).FirstOrDefault();
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string contentRootPath = _webHostEnvironment.ContentRootPath;
                    //string zipPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["StudentfileUploadFolder"]);

                    //string zipFileName = Path.Combine(zipPath, studentDetails.UserName + fileUploadId + ".zip");
                    var path = Path.Combine(webRootPath, "Uploads");
                    var zipFileName = Path.Combine(webRootPath, "Uploads", "Templates", studentDetails.UserName + fileUploadId + ".zip");
                    if (!Directory.Exists(zipFileName))
                    {
                        //var response = new HttpResponseMessage(HttpStatusCode.OK);
                        //var stream = new System.IO.FileStream(zipFileName, System.IO.FileMode.Open);
                        //response.Content = new StreamContent(stream);
                        //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        return Ok(zipFileName);
                    }
                    else
                    {
                        Dictionary<string, byte[]> fileList = new Dictionary<string, byte[]>();
                        foreach (var file in practiceProblemsStudentFiles)
                        {

                            fileList.Add(file.FileName, System.IO.File.ReadAllBytes(path + @"\" + file.FileName));
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                            {
                                foreach (var file in fileList)
                                {
                                    var demoFile = archive.CreateEntry(file.Key);

                                    using (var entryStream = demoFile.Open())
                                    using (var b = new BinaryWriter(entryStream))
                                    {
                                        b.Write(file.Value);
                                    }
                                }
                            }

                            using (var fileStream = new FileStream(zipFileName, FileMode.Create))
                            {
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                memoryStream.CopyTo(fileStream);
                            }
                            //var response = new HttpResponseMessage(HttpStatusCode.OK);
                            //var stream = new System.IO.FileStream(zipFileName, System.IO.FileMode.Open);
                            //response.Content = new StreamContent(stream);
                            //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                            return Ok(zipFileName);
                        }
                    }

                }


                else
                {
                    return BadRequest("Failed to View Student Answers");
                }
            }
            #endregion
            #region mcq

            [HttpGet]
            [Route("mcqList")]
            public async Task<IActionResult> GetAllPracticeProblemsMCQ()
            {
                try
                {
                    if (ModelState.IsValid)
                    {

                        var practiceProblemsMCQ = new List<PracticeProblemsMCQMain>();
                        practiceProblemsMCQ = _db.PracticeProblemsMCQMain.Include(s => s.QuestionDetails).Include(c => c.Classes).Include(s => s.Subjects).ToList();

                        var practiceproblemDto = new List<PracticeProblemsMCQDto>();


                        _mapper.Map(practiceProblemsMCQ, practiceproblemDto);

                        if (practiceproblemDto == null)
                        {
                            return NotFound();
                        }
                        else
                        {

                            return Ok(practiceproblemDto);
                        }


                    }
                    else
                    {
                        return BadRequest();
                    }
                }
                catch (Exception e)
                {

                    throw e;
                }

            }

            [HttpGet]
            [Route("mcq/{practiceProblemId}")]
            public async Task<IActionResult> GetpracticeProblemMCQById(int practiceProblemId)
            {
                if (ModelState.IsValid)
                {

                    if (practiceProblemId == 0)
                    {
                        return BadRequest();
                    }
                    PracticeProblemsMCQMain practiceProblemsMCQ = new PracticeProblemsMCQMain();
                    List<PracticeProblemsMCQDetail> MCQDetailList = new List<PracticeProblemsMCQDetail>();



                    practiceProblemsMCQ = _db.PracticeProblemsMCQMain.Include(q => q.QuestionDetails).Include(c => c.Classes).Include(s => s.Subjects).Where(o => o.Id == practiceProblemId).SingleOrDefault();


                    if (practiceProblemsMCQ == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(practiceProblemsMCQ);
                    }


                }
                else
                {
                    return BadRequest();
                }
            }


            [HttpPost]
            [Route("mcq")]
            public async Task<IActionResult> PostPracticeProblemsMCQ([FromBody] PracticeProblemsMCQDto practiceProblemMCQDto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);
                    }
                    var userId = CurrentUser.Id;
                    bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                    if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                    {
                        List<PracticeProblemsMCQMain> practiceProblemMCQList = await _practiceProblemsMCQ.GetAllAsync();
                        /*if (practiceProblemMCQList.Any(x => x.Name.ToLower().Trim() == practiceProblemMCQDto.Name.ToLower().Trim()))
                            return BadRequest($"Practice Problem {practiceProblemMCQDto.Name} Already Exists");*/

                        PracticeProblemsMCQMain practiceProblemsMCQ = new PracticeProblemsMCQMain();


                        practiceProblemsMCQ.Name = practiceProblemMCQDto.Name;
                        practiceProblemsMCQ.ClassId = practiceProblemMCQDto.ClassId;
                        practiceProblemsMCQ.SubjectId = practiceProblemMCQDto.SubjectId;
                        practiceProblemsMCQ.TopicId = (long)practiceProblemMCQDto.TopicId;
                        practiceProblemsMCQ.CreatedDate = practiceProblemMCQDto.CreatedDate;
                        practiceProblemMCQDto.CreatedOn = practiceProblemsMCQ.CreatedOn;
                        practiceProblemMCQDto.IsActive = true;
                        practiceProblemMCQDto.RefId = practiceProblemsMCQ.RefId;
                        //practiceProblemsMCQ.Status =(int) PracticeTestStatus.NotAttended;

                        PracticeProblemsMCQDetailDto practiceProblemsMCQDetailDto = new PracticeProblemsMCQDetailDto();
                        practiceProblemsMCQ.QuestionDetails = (from item in practiceProblemMCQDto.QuestionDetails
                                                               select new PracticeProblemsMCQDetail()
                                                               {
                                                                   Question = item.Question,
                                                                   IsActive = true,
                                                                   RefId = practiceProblemsMCQDetailDto.RefId,
                                                                   PracticeProblemsMCQId = practiceProblemsMCQ.Id
                                                               }).ToList();

                        practiceProblemsMCQ.CreatedBy = CurrentUser.Id;
                        _db.PracticeProblemsMCQMain.Add(practiceProblemsMCQ);
                        _db.SaveChanges();



                        practiceProblemMCQDto.Id = practiceProblemsMCQ.Id;
                        return Ok(practiceProblemMCQDto);
                    }

                    else
                    {
                        return BadRequest("Not Authorized to perform this action!..");
                    }
                }
                catch (Exception e)
                {

                    throw e;
                }

            }

            [HttpPut]
            [Route("updatemcq")]
            public async Task<IActionResult> UpdatePracticeProblemsMCQ([FromBody] PracticeProblemsMCQDto practiceProblemMCQDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {
                    PracticeProblemsMCQMain practiceProblemsMCQ = new PracticeProblemsMCQMain();
                    List<PracticeProblemsMCQDetail> MCQDetailList = new List<PracticeProblemsMCQDetail>();

                    if (practiceProblemsMCQ == null)
                    {
                        return NotFound();
                    }

                    practiceProblemsMCQ = _db.PracticeProblemsMCQMain.Include(s => s.QuestionDetails).Where(o => o.Id == practiceProblemMCQDto.Id).SingleOrDefault();

                    practiceProblemsMCQ.Name = practiceProblemMCQDto.Name;
                    practiceProblemsMCQ.ClassId = practiceProblemMCQDto.ClassId;
                    practiceProblemsMCQ.SubjectId = practiceProblemMCQDto.SubjectId;
                    practiceProblemsMCQ.TopicId = (long)practiceProblemMCQDto.TopicId;
                    practiceProblemsMCQ.CreatedDate = practiceProblemMCQDto.CreatedDate;

                    foreach (var existingChild in practiceProblemsMCQ.QuestionDetails.ToList())
                    {
                        _db.PracticeProblemsMCQDetails.Remove(existingChild);
                    }

                    PracticeProblemsMCQDetailDto practiceProblemsMCQDetailDto = new PracticeProblemsMCQDetailDto();
                    MCQDetailList = (from item in practiceProblemMCQDto.QuestionDetails
                                     select new PracticeProblemsMCQDetail()
                                     {
                                         Question = item.Question,
                                         IsActive = true,
                                         RefId = practiceProblemsMCQDetailDto.RefId,
                                         PracticeProblemsMCQId = practiceProblemsMCQ.Id
                                     }).ToList();
                    practiceProblemsMCQ.QuestionDetails = MCQDetailList;
                    practiceProblemsMCQ.UpdatedBy = CurrentUser.Id;
                    _db.SaveChanges();


                    await _practiceProblemsMCQ.UpdateAsync(practiceProblemsMCQ);
                    _db.SaveChanges();

                    var practiceproblemsMCQDtoupdate = new PracticeProblemsMCQDto();
                    _mapper.Map(practiceProblemsMCQ, practiceproblemsMCQDtoupdate);
                    return Ok(practiceproblemsMCQDtoupdate);
                }
                else
                {
                    return BadRequest("Not Authorized to perform this action!..");
                }
            }

            [HttpDelete]
            [Route("deletemcq")]
            public async Task<IActionResult> DeletePracticeProblemMCQ(long Id)
            {

                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {

                    PracticeProblemsMCQMain practiceProblemsMCQ = new PracticeProblemsMCQMain();

                    practiceProblemsMCQ = _db.PracticeProblemsMCQMain.Include(s => s.QuestionDetails).Where(o => o.Id == Id).SingleOrDefault();
                    if (practiceProblemsMCQ != null)

                        await _practiceProblemsMCQ.DeleteAsync(practiceProblemsMCQ);
                    return Ok("Practice Problem  deleted successfully");
                }


                else
                {
                    return BadRequest("Your Not Authorized to Delete Practice Problem");
                }
            }
            #endregion

            #region MCQ Student 

            [HttpGet]
            [Route("mcqStudentView")]
            public async Task<IActionResult> GetpracticeProblemMCQStudentView(int classId, int subjectId)
            {
                if (ModelState.IsValid)
                {

                    if (classId == 0 && subjectId == 0)
                    {
                        return BadRequest();
                    }
                    List<PracticeProblemsMCQMain> practiceProblemsMCQList = new List<PracticeProblemsMCQMain>();
                    List<PracticeProblemsMCQDetail> MCQDetailList = new List<PracticeProblemsMCQDetail>();

                    practiceProblemsMCQList = _db.PracticeProblemsMCQMain.Include(q => q.QuestionDetails).Include(c => c.Classes).Include(s => s.Subjects).Where(o => o.ClassId == classId && o.SubjectId == subjectId).ToList();
                    var practiceproblemDto = new List<PracticeProblemsMCQDto>();
                    _mapper.Map(practiceProblemsMCQList, practiceproblemDto);

                    if (practiceproblemDto == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok(practiceproblemDto);
                    }


                }
                else
                {
                    return BadRequest();
                }
            }
            [HttpPost]
            [Route("mcqStudentAnswers")]
            public async Task<IActionResult> PostPracticeProblemsMCQStudentAnswers([FromBody] PracticeProblemsMCQStudentAnswersDto practiceProblemMCQAnswerDto)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Individual Student With Package Subscription") || CurrentUser.IsInRole("Student") || User.IsInRole("Individual Student With Assignment Rights")) && checkTSSchoolUser)
                {
                    List<PracticeProblemsMCQAnswersMain> practiceProblemMCQAnswerList = await _practiceProblemsMcqAnswers.GetAllAsync();
                    if (practiceProblemMCQAnswerList.Any(x => x.StudentId.ToLower().Trim() == userId.ToString() && x.McqMainId == practiceProblemMCQAnswerDto.Id))
                    {
                        return BadRequest($"Your already Answered for this practice problem!.");
                    }
                    else
                    {
                        PracticeProblemsMCQAnswersMain practiceProblemsMCQAnswers = new PracticeProblemsMCQAnswersMain();
                        practiceProblemsMCQAnswers.StudentId = userId.ToString();
                        practiceProblemsMCQAnswers.ClassId = practiceProblemMCQAnswerDto.ClassId;
                        practiceProblemsMCQAnswers.SubjectId = practiceProblemMCQAnswerDto.SubjectId;
                        practiceProblemsMCQAnswers.McqMainId = (long)practiceProblemMCQAnswerDto.Id;
                        practiceProblemMCQAnswerDto.CreatedOn = practiceProblemsMCQAnswers.CreatedOn;
                        practiceProblemMCQAnswerDto.IsActive = true;
                        practiceProblemMCQAnswerDto.RefId = practiceProblemsMCQAnswers.RefId;
                        practiceProblemsMCQAnswers.Status = (int)PracticeTestStatus.Attended;

                        PracticeProblemsMCQStudentAnswersDetailDto practiceProblemsMCQDetailDto = new PracticeProblemsMCQStudentAnswersDetailDto();
                        practiceProblemsMCQAnswers.AnswerDetails = (from item in practiceProblemMCQAnswerDto.AnswerDetails
                                                                    select new PracticeProblemsMCQAnswersDetail()
                                                                    {
                                                                        MCQAnswerMainId = practiceProblemsMCQAnswers.Id,
                                                                        McqQuestionId = (long)item.Id,
                                                                        Answer = item.Answer,
                                                                        IsActive = true,
                                                                        RefId = practiceProblemsMCQDetailDto.RefId,

                                                                    }).ToList();

                        practiceProblemsMCQAnswers.CreatedBy = CurrentUser.Id;
                        _db.PracticeProblemsMCQStudentAnswersMain.Add(practiceProblemsMCQAnswers);
                        _db.SaveChanges();
                        PracticeProblemsMCQMain practiceProblemsMCQ = new PracticeProblemsMCQMain();

                        //await _practiceProblemsMCQ.UpdateAsync(practiceProblemsMCQ);
                        // _db.SaveChanges();



                        practiceProblemMCQAnswerDto.Id = practiceProblemsMCQAnswers.Id;
                        return Ok(practiceProblemMCQAnswerDto);
                    }

                }

                else
                {
                    return BadRequest("Not Authorized to perform this action!..");
                }
            }


            [HttpGet]
            [Route("mcqAnsweredStudentsList")]
            public async Task<IActionResult> PracticeProblemViewMcqStudentAnswers(long mcqId, long classId, long subjectId)
            {


                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {


                    var answersListDtos = (from user in _db.Users
                                           join pps in _db.PracticeProblemsMCQStudentAnswersMain on user.Id equals pps.StudentId
                                           where pps.McqMainId == mcqId && pps.ClassId == classId && pps.SubjectId == subjectId
                                           select new
                                           {
                                               Name = user.FirstName + "  " + user.LastName,
                                               className = pps.Classes.Name,
                                               pps.ClassId,
                                               pps.SubjectId,
                                               SubjectName = pps.Subjects.Name,
                                               pps.StudentId,
                                               pps.McqMainId,
                                               pps.Status,

                                           })
                                                .GroupBy(n => n.StudentId).Select(grp => grp.FirstOrDefault()).ToList();

                    if (answersListDtos.Any() == false)
                    {
                        return Ok("");
                    }
                    else
                    {
                        return Ok(answersListDtos);
                    }


                }

                else
                {
                    return BadRequest("Failed to View Student Answers");
                }
            }

            [HttpGet]
            [Route("mcqAnsweredStudentById")]
            //subject teacher can view individual student answers 
            public async Task<IActionResult> mcqAnsweredStudentById(string studentId, long mcqId, long classId, long subjectId)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {


                    var answersListDtos = (from s in _db.PracticeProblemsMCQDetails
                                           join a in _db.PracticeProblemsMCQAnswersDetail on s.Id equals a.McqQuestionId

                                           join q in _db.PracticeProblemsMCQStudentAnswersMain on a.MCQAnswerMainId equals q.Id
                                           where q.StudentId == studentId && q.ClassId == classId && q.SubjectId == subjectId && q.McqMainId == mcqId
                                           select new PracticeProblems
                                           {
                                               McqMainId = q.Id.ToString(),
                                               question = s.Question,
                                               answer = a.Answer,
                                               MCQAnswemainid = a.MCQAnswerMainId.ToString(),
                                               MCQquestionid = a.McqQuestionId.ToString(),
                                               Isvalid = a.Isvalid == null ? false : a.Isvalid,
                                               Id = a.Id.ToString()
                                           }).GroupBy(n => n.question).Select(grp => grp.FirstOrDefault()).ToList();


                    if (answersListDtos.Any() == false)
                    {
                        return BadRequest("Not Found");
                    }
                    else
                    {
                        return Ok(answersListDtos);
                    }




                }

                else
                {
                    return BadRequest("Failed to View Student Answers");
                }
            }

            #endregion


            #region fileupload student



            [HttpGet]
            [Route("fileUploadStudentView")]
            public async Task<IActionResult> GetpracticeProblemFileUploadStudent(int? classId, int? subjectId)
            {
                if (ModelState.IsValid)
                {

                    if (classId == null && subjectId == null)
                    {
                        return BadRequest();
                    }


                    List<PracticeProblemsFileUpload> practiceProblemsFiles = _db.PracticeProblemsFileUploadMain.Include(s => s.FileDetails).Include(m => m.StudentFileDetails).Include(c => c.Classes).Include(s => s.Subjects).Where(o => o.ClassId == classId && o.SubjectId == subjectId).ToList();
                    var practiceProblemsFilesuploaddto = new List<PracticeProblemsFileUploadDto>();
                    _mapper.Map(practiceProblemsFiles, practiceProblemsFilesuploaddto);

                    if (practiceProblemsFilesuploaddto == null)
                    {
                        return NotFound();
                    }
                    //
                    else
                        return Ok(practiceProblemsFilesuploaddto);

                }
                else
                {
                    return BadRequest();
                }
            }
            private static readonly HashSet<string> validExtensions = new HashSet<string>()
        {
            ".png",
            ".jpg",
            ".jpeg",
            ".pdf",
            ".docx",
            ".PNG",
            ".JPG",
            ".JPEG",
            ".PDF",
            ".DOCX"
            // Other possible extensions
        };
            public static bool IsValidExtension(string ext)
            {
                return validExtensions.Contains(ext);
            }

            [HttpPost]
            [Route("StudentFileUpload")]
            public async Task<IActionResult> PostPracticeProblemsfileUploadStudent(long FileUploadId, long ClassId, long SubjectId, DateTime CreatedDate)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;

                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);

                PracticeProblemsStudentFileUpload fileUploadList = new PracticeProblemsStudentFileUpload();

                if (checkTSSchoolUser)
                {

                    using (var _context = new AppDbContext())
                    {

                        try
                        {
                            List<PracticeProblemsStudentFileUpload> practiceProblemsStudentFiles = _context.PracticeProblemsStudentFileUploads.Where(o => o.FileUploadMainId == FileUploadId).Where(s => s.StudentId == userId.ToString()).ToList();

                            if (practiceProblemsStudentFiles.Count > 0)
                            {
                                return BadRequest("Your Already Uploaded the Answers");
                            }

                            else
                            {

                                var httpPostedFile = _httpContextAccessor.HttpContext.Request.Form.Files.Count > 0 ? _httpContextAccessor.HttpContext.Request.Form.Files[0] : null;

                                var postedFile = _httpContextAccessor.HttpContext.Request.Form.Files;

                                if (httpPostedFile != null && httpPostedFile.Length > 0)
                                {

                                    for (int i = 0; i < postedFile.Count; i++)

                                    {
                                        var fileName = Path.GetFileName(postedFile[i].FileName);
                                        string fileNameExtension = System.IO.Path.GetExtension(postedFile[i].FileName);
                                        if (IsValidExtension(fileNameExtension))
                                        {
                                            fileUploadList = new PracticeProblemsStudentFileUpload()
                                            {
                                                FileName = fileName,
                                                FileExtension = fileNameExtension,
                                                ClassId = ClassId,
                                                SubjectId = SubjectId,
                                                FileUploadMainId = FileUploadId,
                                                StudentId = userId.ToString(),
                                                CreatedDate = CreatedDate,
                                                IsActive = true,
                                                CreatedBy = CurrentUser.Id
                                            };
                                            _context.PracticeProblemsStudentFileUploads.Add(fileUploadList);
                                        }
                                        else
                                        {
                                            return BadRequest(fileName + "is not valid");
                                        }
                                        string webRootPath = _webHostEnvironment.WebRootPath;
                                        string contentRootPath = _webHostEnvironment.ContentRootPath;



                                        var path = Path.Combine(webRootPath, "Uploads");
                                        // var path = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["StudentfileUploadFolder"]);
                                        if (!Directory.Exists(path))
                                        {
                                            Directory.CreateDirectory(path);
                                        }

                                        // Construct file save path  
                                        //var fileSavePath = Path.Combine(HostingEnvironment.MapPath(ConfigurationManager.AppSettings["StudentfileUploadFolder"]), fileName);
                                        var fileSavePath = Path.Combine(webRootPath, "Uploads", "Templates", fileName);
                                        //if (File.Exists(fileSavePath))
                                        //{
                                        //    File.Delete(fileSavePath);
                                        //}
                                        if (Directory.Exists(path))
                                        {
                                            Directory.Delete(path);
                                        }

                                        //Save the uploaded file
                                        using (var fileStream = new FileStream(fileSavePath, FileMode.Create))
                                        {
                                            await postedFile[i].CopyToAsync(fileStream);
                                        }
                                        //postedFile[i].SaveAs(fileSavePath);


                                    }


                                }

                                _context.SaveChanges();

                                return Ok("File Uploaded Successfully");
                            }



                        }
                        catch (Exception ex)
                        {
                            return BadRequest(ex.Message.ToString());
                        }

                    }

                }

                else
                {
                    return BadRequest("User not valid");
                }
            }

            [HttpPost]
            [Route("StudentDownloadZipFile")]
            public async Task<IActionResult> PostPracticeProblemsStudentDownloadZipFile(long FileUploadId)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                try
                {
                    var userId = CurrentUser.Id;
                    bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);

                    if (checkTSSchoolUser)
                    {

                        PracticeProblemsFileUpload practiceProblemsFiles = _db.PracticeProblemsFileUploadMain.Include(s => s.FileDetails).Where(o => o.Id == FileUploadId).SingleOrDefault();
                        if (practiceProblemsFiles == null)
                        {
                            return BadRequest("Not Found");
                        }
                        //string zipPath = HostingEnvironment.MapPath(ConfigurationManager.AppSettings["fileUploadFolder"]);

                        //string zipFileName = Path.Combine(zipPath, practiceProblemsFiles.Name + ".zip");

                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string contentRootPath = _webHostEnvironment.ContentRootPath;



                        var zipPath = Path.Combine(webRootPath, "Uploads");
                        var zipFileName = Path.Combine(webRootPath, "Uploads", "Templates", practiceProblemsFiles.Name + ".zip");
                        //if (File.Exists(zipFileName))
                        //{
                        //    //var response = new HttpResponseMessage(HttpStatusCode.OK);
                        //    //var stream = new System.IO.FileStream(zipFileName, System.IO.FileMode.Open);
                        //    //response.Content = new StreamContent(stream);
                        //    //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                        //    return Ok(zipFileName);
                        //}
                        if (Directory.Exists(zipFileName))
                        {
                            return Ok(zipFileName);
                        }
                        else
                        {
                            Dictionary<string, byte[]> fileList = new Dictionary<string, byte[]>();
                            foreach (var file in practiceProblemsFiles.FileDetails)
                            {
                                fileList.Add(file.FileName, System.IO.File.ReadAllBytes(zipPath + @"\" + file.FileName));
                            }

                            //ZipFile.CreateFromDirectory(zipPath, zipFileName);
                            using (var memoryStream = new MemoryStream())
                            {
                                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                                {
                                    foreach (var file in fileList)
                                    {
                                        var demoFile = archive.CreateEntry(file.Key);

                                        using (var entryStream = demoFile.Open())
                                        using (var b = new BinaryWriter(entryStream))
                                        {
                                            b.Write(file.Value);
                                        }
                                    }
                                }

                                using (var fileStream = new FileStream(zipFileName, FileMode.Create))
                                {
                                    memoryStream.Seek(0, SeekOrigin.Begin);
                                    memoryStream.CopyTo(fileStream);
                                }
                                //var response = new HttpResponseMessage(HttpStatusCode.OK);
                                //var stream = new System.IO.FileStream(zipFileName, System.IO.FileMode.Open);
                                //response.Content = new StreamContent(stream);
                                //response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                                return Ok(zipFileName);
                            }
                        }

                    }



                    else
                    {
                        return BadRequest("User not valid");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message.ToString());
                }


            }

            [HttpGet]

            [Route("GetReportbyPracticeClass")]

            public async Task<IActionResult> GetReportbyPracticeClass(int Id)
            {
                try
                {

                    var userId = CurrentUser.Id;
                    bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);

                    if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                    {



                        var res = _classScheduleService.GetReportbyPractiseClass(Id);
                        return Ok(res);

                    }

                    else
                    {
                        return BadRequest("Not Authorized to perform this action!..");
                    }
                }
                catch (Exception ex)
                {
                    //return (IActionResult)Request.CreateErrorResponse(HttpStatusCode.ExpectationFailed, ex.Message);
                    return BadRequest(ex.Message.ToString());

                }
            }


            [HttpPost]
            [Route("UpdateMCQStudentAnswer")]
            //subject teacher can view individual student answers 
            public async Task<IActionResult> UpdateMCQStudentAnswer([FromBody] List<PracticeProblems> obj)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var userId = CurrentUser.Id;
                bool checkTSSchoolUser = _classScheduleService.CheckUserExistsTalentSpireSchool(userId);
                if ((CurrentUser.IsInRole("Subject Teacher") || CurrentUser.IsInRole("TalentSpire Admin")) && checkTSSchoolUser)
                {

                    var McqMainId = obj.Select(m => m.McqMainId).FirstOrDefault();
                    var Mcqmain = _db.PracticeProblemsMCQStudentAnswersMain.Where(m => m.Id.ToString() == McqMainId.ToString()).FirstOrDefault();
                    Mcqmain.Status = (int)PracticeTestStatus.Reviewed;
                    _db.Entry(Mcqmain).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    _db.SaveChanges();
                    obj.ForEach(m =>
                    {

                        var update = _db.PracticeProblemsMCQAnswersDetail.Where(a => a.Id.ToString() == m.Id && a.MCQAnswerMainId.ToString() == m.MCQAnswemainid && a.McqQuestionId.ToString() == m.MCQquestionid).FirstOrDefault();
                        update.Isvalid = m.Isvalid;
                        _db.Entry(update).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        _db.SaveChanges();

                    });


                    return Ok();

                }


                else
                {
                    return BadRequest("Failed to View Student Answers");
                }
            }


            #endregion
        }
    }

}
