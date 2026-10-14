using System.Globalization;

namespace DishManagement;

/// <summary>
/// Bộ dịch giao diện đơn giản English / Tiếng Việt (không dùng thư viện ngoài).
/// Ngôn ngữ hiện tại được lưu theo từng request (AsyncLocal) từ cookie "lang".
/// </summary>
public static class L
{
    private static readonly AsyncLocal<string> _lang = new();

    public static string Lang
    {
        get => _lang.Value ?? "en";
        set => _lang.Value = value == "vi" ? "vi" : "en";
    }

    public static bool IsVi => Lang == "vi";

    public static string T(string key, params object[] args)
    {
        var dict = IsVi ? Vi : En;
        if (!dict.TryGetValue(key, out var s) && !En.TryGetValue(key, out s)) s = key;
        return args.Length > 0 ? string.Format(CultureInfo.InvariantCulture, s, args) : s;
    }

    public static string Money(decimal price)
    {
        var s = price.ToString("#,##0.##", CultureInfo.InvariantCulture);
        if (IsVi) s = s.Replace(',', '#').Replace('.', ',').Replace('#', '.');
        return s + " ₫";
    }

    private static readonly Dictionary<string, string> En = new()
    {
        ["app.title"] = "Dish Management",
        ["nav.dishes"] = "Dishes",
        ["nav.categories"] = "Dish categories",
        ["nav.language"] = "Language",

        ["btn.create"] = "Create new",
        ["btn.save"] = "Save",
        ["btn.cancel"] = "Cancel",
        ["btn.back"] = "Back to list",
        ["btn.edit"] = "Edit",
        ["btn.details"] = "Details",
        ["btn.delete"] = "Delete",
        ["btn.filter"] = "Search / Filter",
        ["btn.reset"] = "Reset",
        ["btn.addImage"] = "Add image",
        ["btn.setThumb"] = "Set as thumbnail",
        ["btn.manageImages"] = "Manage images",

        ["field.name"] = "Name",
        ["field.price"] = "Price",
        ["field.time"] = "Preparation time (minutes)",
        ["field.available"] = "Available (on sale)",
        ["field.description"] = "Description",
        ["field.category"] = "Category",
        ["field.imageUrl"] = "Image URL",
        ["field.status"] = "Status",
        ["field.timeClass"] = "Time class",
        ["field.dishCount"] = "Number of dishes",
        ["field.categoryName"] = "Category name",
        ["field.actions"] = "Actions",
        ["field.image"] = "Image",
        ["field.thumbnail"] = "Use as thumbnail",

        ["filter.search"] = "Search by dish name",
        ["filter.category"] = "Category",
        ["filter.allCategories"] = "All categories",
        ["filter.status"] = "Status",
        ["filter.allStatuses"] = "All statuses",
        ["filter.minPrice"] = "Price from",
        ["filter.maxPrice"] = "Price to",
        ["filter.sort"] = "Sort by",
        ["sort.name"] = "Name A-Z",
        ["sort.priceAsc"] = "Price: low to high",
        ["sort.priceDesc"] = "Price: high to low",
        ["sort.timeAsc"] = "Preparation time: shortest first",

        ["status.available"] = "Available",
        ["status.unavailable"] = "Unavailable",
        ["time.quick"] = "Quick",
        ["time.regular"] = "Regular",
        ["minutes"] = "{0} min",
        ["badge.thumbnail"] = "Thumbnail",

        ["title.dishList"] = "Dish list",
        ["title.dishCreate"] = "Create dish",
        ["title.dishEdit"] = "Edit dish",
        ["title.dishDetails"] = "Dish details",
        ["title.categoryList"] = "Dish categories",
        ["title.categoryCreate"] = "Create dish category",
        ["title.categoryEdit"] = "Edit dish category",
        ["title.images"] = "Images",
        ["title.addImage"] = "Add image by URL",
        ["title.notFound"] = "Not found",
        ["title.error"] = "Error",

        ["msg.created"] = "Created successfully.",
        ["msg.updated"] = "Updated successfully.",
        ["msg.deleted"] = "Deleted successfully.",
        ["msg.imageAdded"] = "Image added successfully.",
        ["msg.thumbSet"] = "Thumbnail updated successfully.",
        ["msg.imageDeleted"] = "Image deleted successfully.",

        ["empty.dishes"] = "No dishes found.",
        ["empty.categories"] = "No categories yet.",
        ["empty.images"] = "This dish has no images yet.",
        ["noImage"] = "No image",
        ["confirm.delete"] = "Are you sure you want to delete this item?",
        ["select.category"] = "-- Select category --",
        ["warn.noCategory"] = "No category exists yet. Please create a category first.",

        ["err.dishNotFound"] = "Dish not found. The Id does not exist.",
        ["err.categoryNotFound"] = "Dish category not found. The Id does not exist.",
        ["err.imageNotFound"] = "Dish image not found. The Id does not exist.",
        ["err.pageNotFound"] = "The page you requested does not exist.",
        ["err.priceRange"] = "Invalid price range: prices must not be negative and the minimum must not be greater than the maximum.",
        ["err.categoryInUse"] = "Cannot delete this category because it is being used by one or more dishes.",
        ["err.imageUrlInvalid"] = "Image URL must be a valid http/https address (or a path starting with /).",
        ["err.generic"] = "Something went wrong. Please try again.",
        ["err.unexpected"] = "An unexpected error occurred.",
        ["err.duplicateDish"] = "A dish with this name already exists in the selected category.",
        ["err.duplicateCategory"] = "A category with this name already exists.",
        ["err.filterCategoryNotFound"] = "The selected category does not exist. The category filter was ignored.",

        ["val.required"] = "This field is required.",
        ["val.number"] = "Please enter a valid number.",
        ["val.invalidValue"] = "The entered value is invalid.",
        ["val.name.required"] = "Name must not be empty.",
        ["val.name.max"] = "Name must not exceed {0} characters.",
        ["val.price.positive"] = "Price must be greater than 0.",
        ["val.time.positive"] = "Preparation time must be greater than 0.",
        ["val.desc.max"] = "Description must not exceed {0} characters.",
        ["val.tooLarge"] = "The value is too large (maximum {0}).",
        ["val.category.notFound"] = "The selected category does not exist.",
        ["val.category.name.required"] = "Category name must not be empty.",
        ["val.category.name.max"] = "Category name must not exceed {0} characters.",
        ["val.image.required"] = "Image URL must not be empty.",
    };

    private static readonly Dictionary<string, string> Vi = new()
    {
        ["app.title"] = "Quản lý món ăn",
        ["nav.dishes"] = "Món ăn",
        ["nav.categories"] = "Loại món ăn",
        ["nav.language"] = "Ngôn ngữ",

        ["btn.create"] = "Thêm mới",
        ["btn.save"] = "Lưu",
        ["btn.cancel"] = "Hủy",
        ["btn.back"] = "Quay lại danh sách",
        ["btn.edit"] = "Sửa",
        ["btn.details"] = "Chi tiết",
        ["btn.delete"] = "Xóa",
        ["btn.filter"] = "Tìm kiếm / Lọc",
        ["btn.reset"] = "Đặt lại",
        ["btn.addImage"] = "Thêm ảnh",
        ["btn.setThumb"] = "Đặt làm ảnh đại diện",
        ["btn.manageImages"] = "Quản lý ảnh",

        ["field.name"] = "Tên món",
        ["field.price"] = "Giá",
        ["field.time"] = "Thời gian chế biến (phút)",
        ["field.available"] = "Đang bán",
        ["field.description"] = "Mô tả",
        ["field.category"] = "Loại món",
        ["field.imageUrl"] = "Đường dẫn ảnh",
        ["field.status"] = "Trạng thái",
        ["field.timeClass"] = "Phân loại thời gian",
        ["field.dishCount"] = "Số món",
        ["field.categoryName"] = "Tên loại món",
        ["field.actions"] = "Thao tác",
        ["field.image"] = "Ảnh",
        ["field.thumbnail"] = "Đặt làm ảnh đại diện",

        ["filter.search"] = "Tìm theo tên món",
        ["filter.category"] = "Loại món",
        ["filter.allCategories"] = "Tất cả loại món",
        ["filter.status"] = "Trạng thái",
        ["filter.allStatuses"] = "Tất cả trạng thái",
        ["filter.minPrice"] = "Giá từ",
        ["filter.maxPrice"] = "Giá đến",
        ["filter.sort"] = "Sắp xếp",
        ["sort.name"] = "Tên A-Z",
        ["sort.priceAsc"] = "Giá tăng dần",
        ["sort.priceDesc"] = "Giá giảm dần",
        ["sort.timeAsc"] = "Thời gian chế biến tăng dần",

        ["status.available"] = "Đang bán",
        ["status.unavailable"] = "Ngừng bán",
        ["time.quick"] = "Nhanh",
        ["time.regular"] = "Thông thường",
        ["minutes"] = "{0} phút",
        ["badge.thumbnail"] = "Ảnh đại diện",

        ["title.dishList"] = "Danh sách món ăn",
        ["title.dishCreate"] = "Thêm món ăn",
        ["title.dishEdit"] = "Sửa món ăn",
        ["title.dishDetails"] = "Chi tiết món ăn",
        ["title.categoryList"] = "Danh sách loại món ăn",
        ["title.categoryCreate"] = "Thêm loại món ăn",
        ["title.categoryEdit"] = "Sửa loại món ăn",
        ["title.images"] = "Hình ảnh",
        ["title.addImage"] = "Thêm ảnh bằng đường dẫn",
        ["title.notFound"] = "Không tìm thấy",
        ["title.error"] = "Lỗi",

        ["msg.created"] = "Tạo mới thành công.",
        ["msg.updated"] = "Cập nhật thành công.",
        ["msg.deleted"] = "Xóa thành công.",
        ["msg.imageAdded"] = "Thêm ảnh thành công.",
        ["msg.thumbSet"] = "Đã cập nhật ảnh đại diện.",
        ["msg.imageDeleted"] = "Đã xóa ảnh.",

        ["empty.dishes"] = "Không tìm thấy món ăn nào.",
        ["empty.categories"] = "Chưa có loại món ăn nào.",
        ["empty.images"] = "Món ăn này chưa có ảnh.",
        ["noImage"] = "Chưa có ảnh",
        ["confirm.delete"] = "Bạn có chắc chắn muốn xóa mục này không?",
        ["select.category"] = "-- Chọn loại món --",
        ["warn.noCategory"] = "Chưa có loại món nào. Vui lòng tạo loại món trước.",

        ["err.dishNotFound"] = "Không tìm thấy món ăn. Id không tồn tại.",
        ["err.categoryNotFound"] = "Không tìm thấy loại món ăn. Id không tồn tại.",
        ["err.imageNotFound"] = "Không tìm thấy ảnh món ăn. Id không tồn tại.",
        ["err.pageNotFound"] = "Trang bạn yêu cầu không tồn tại.",
        ["err.priceRange"] = "Khoảng giá không hợp lệ: giá không được âm và giá từ không được lớn hơn giá đến.",
        ["err.categoryInUse"] = "Không thể xóa loại món này vì đang có món ăn sử dụng.",
        ["err.imageUrlInvalid"] = "Đường dẫn ảnh phải là địa chỉ http/https hợp lệ (hoặc đường dẫn bắt đầu bằng /).",
        ["err.generic"] = "Đã xảy ra lỗi. Vui lòng thử lại.",
        ["err.unexpected"] = "Đã xảy ra lỗi không mong muốn.",
        ["err.duplicateDish"] = "Tên món ăn đã tồn tại trong loại món đã chọn.",
        ["err.duplicateCategory"] = "Tên loại món đã tồn tại.",
        ["err.filterCategoryNotFound"] = "Loại món đã chọn không tồn tại. Bộ lọc loại món đã bị bỏ qua.",

        ["val.required"] = "Trường này là bắt buộc.",
        ["val.number"] = "Vui lòng nhập một số hợp lệ.",
        ["val.invalidValue"] = "Giá trị nhập vào không hợp lệ.",
        ["val.name.required"] = "Tên không được để trống.",
        ["val.name.max"] = "Tên không được vượt quá {0} ký tự.",
        ["val.price.positive"] = "Giá phải lớn hơn 0.",
        ["val.time.positive"] = "Thời gian chế biến phải lớn hơn 0.",
        ["val.desc.max"] = "Mô tả không được vượt quá {0} ký tự.",
        ["val.tooLarge"] = "Giá trị quá lớn (tối đa {0}).",
        ["val.category.notFound"] = "Loại món đã chọn không tồn tại.",
        ["val.category.name.required"] = "Tên loại món không được để trống.",
        ["val.category.name.max"] = "Tên loại món không được vượt quá {0} ký tự.",
        ["val.image.required"] = "Đường dẫn ảnh không được để trống.",
    };
}
