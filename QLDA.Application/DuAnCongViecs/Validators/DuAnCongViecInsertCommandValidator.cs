using QLDA.Application.DuAnCongViecs.Commands;

namespace QLDA.Application.DuAnCongViecs.Validators;

public class DuAnCongViecInsertCommandValidator : AbstractValidator<DuAnCongViecInsertCommand> {
    public DuAnCongViecInsertCommandValidator() {
        RuleFor(x => x.Dto.DuAnId)
            .NotEmpty()
            .WithMessage("ID dự án không được trống");

        RuleFor(x => x.Dto.CongViecId)
            .GreaterThan(0)
            .WithMessage("ID công việc phải lớn hơn 0");
    }
}