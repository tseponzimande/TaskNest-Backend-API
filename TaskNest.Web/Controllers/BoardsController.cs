namespace TaskNest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardsController(IBoardService boardService, IMapper mapper, IBoardUserService _boardUserService) : ControllerBase
    {
        private readonly IBoardService _boardService = boardService;
        private readonly IMapper _mapper = mapper;
        private readonly IBoardUserService _boardUserService = _boardUserService;

        /// <summary>
        /// Get all boards for the current user
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status401Unauthorized)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BoardDto>>> GetBoards()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var allBoards = await _boardService.GetAllAsync();
                var userBoards = new List<Board>();

                foreach (var board in allBoards)
                {
                    var isUserInBoard = await _boardUserService.IsUserInBoardAsync(board.Id, userId);
                    if (isUserInBoard)
                    {
                        userBoards.Add(board);
                    }
                }

                var boardDtos = _mapper.Map<IEnumerable<BoardDto>>(userBoards);
                return Ok(boardDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving boards", detail = ex.Message });
            }
        }

        /// <summary>
        /// Get a specific board by ID
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status403Forbidden)]
        [HttpGet("{id}")]
        public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var board = await _boardService.GetByIdAsync(id);
                if (board == null)
                {
                    return NotFound();
                }

                var isUserInBoard = await _boardUserService.IsUserInBoardAsync(id, userId);
                if (!isUserInBoard)
                {
                    return Forbid("You don't have access to this board.");
                }

                var boardDto = _mapper.Map<BoardDto>(board);
                return Ok(boardDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving board", detail = ex.Message });
            }
        }

        /// <summary>
        /// Create a new board
        /// </summary>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<BoardDto>> CreateBoard(BoardDto dto)
        {
            try
            {
                var board = _mapper.Map<Board>(dto);
                board.Id = Guid.NewGuid();
                var created = await _boardService.CreateAsync(board);

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

                if (string.IsNullOrEmpty(userEmail))
                {
                    var userManager = HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByIdAsync(userId);
                    userEmail = user?.Email;
                }

                await _boardUserService.AddUserToBoardAsync(created.Id, userEmail, BoardUserRole.Admin);

                var createdDto = _mapper.Map<BoardDto>(created);
                return CreatedAtAction(nameof(GetBoard), new { id = createdDto.Id }, createdDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating board", detail = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing board
        /// </summary>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoard(Guid id, BoardDto dto)
        {
            try
            {
                if (id != dto.Id)
                {
                    return BadRequest("ID mismatch");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userRole = await _boardUserService.GetUserRoleInBoardAsync(id, userId);
                if (userRole != BoardUserRole.Admin && userRole != BoardUserRole.Editor)
                {
                    return Forbid("You don't have permission to edit this board.");
                }

                var entity = _mapper.Map<Board>(dto);
                var updated = await _boardService.UpdateAsync(entity);

                return updated ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating board", detail = ex.Message });
            }
        }

        /// <summary>
        /// Delete a board by ID
        /// </summary>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoard(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var userRole = await _boardUserService.GetUserRoleInBoardAsync(id, userId);
                if (userRole != BoardUserRole.Admin)
                {
                    return Forbid("Only board admins can delete boards.");
                }

                var deleted = await _boardService.DeleteAsync(id);
                return deleted ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting board", detail = ex.Message });
            }
        }
    }
}