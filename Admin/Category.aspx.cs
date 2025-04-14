using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecommerce.Admin
{
    public partial class Category : System.Web.UI.Page
    {
        SqlConnection con;
        SqlCommand cmd;
        SqlDataAdapter sda;
        DataTable dt;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMsg.Visible = false;
                hfCategoryId.Value = "0"; // Default to insert mode
                getCategories();
                if (rCategory.Items.Count == 0)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "No categories found.";
                    lblMsg.CssClass = "alert alert-warning";
                }

            }
        }
        void getCategories()
        {
            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("Category_Crud", con);
            cmd.Parameters.AddWithValue("@Action", "GETALL");
            cmd.CommandType = CommandType.StoredProcedure;
            sda= new SqlDataAdapter(cmd);
            dt= new DataTable();
            sda.Fill(dt);
            rCategory.DataSource = dt;
            rCategory.DataBind();

        }


        protected void btnAddOrUpdate_Click(object sender, EventArgs e)
        {
            string actionName = string.Empty, imagePath = string.Empty;
            int categoryId = Convert.ToInt32(hfCategoryId.Value);

            con = new SqlConnection(Utils.getConnection());
            cmd = new SqlCommand("Category_Crud", con);
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameters
            cmd.Parameters.AddWithValue("@Action", categoryId == 0 ? "INSERT" : "UPDATE");
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@CategoryName", txtCategoryName.Text.Trim());
            cmd.Parameters.AddWithValue("@IsActive", cbIsActive.Checked);

            if (fuCategoryImage.HasFile)
            {
                if (Utils.isValidExtension(fuCategoryImage.FileName))
                {
                    string newImageName = Utils.getUniqueId();
                    string fileExtension = Path.GetExtension(fuCategoryImage.FileName);
                    imagePath = "Images/Category/" + newImageName + fileExtension;
                    fuCategoryImage.PostedFile.SaveAs(Server.MapPath("~/Images/Category/" + newImageName + fileExtension));
                    cmd.Parameters.AddWithValue("@CategoryImageUrl", imagePath);
                }
                else
                {
                    ShowMessage("Please select .jpg, .jpeg or .png image", false);
                    return;
                }
            }
            else if (categoryId == 0)
            {
                ShowMessage("Please select an image for new category", false);
                return;
            }

            try
            {
                con.Open();

                // Execute the command
                cmd.ExecuteNonQuery();

                // If we get here, it was successful
                actionName = categoryId == 0 ? "added" : "updated";
                ShowMessage($"Category {actionName} successfully!", true);

                if (fuCategoryImage.HasFile)
                {
                    imagePreview.ImageUrl = imagePath;
                }

                if (categoryId == 0)
                {
                    clear();
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error: " + ex.Message, false);
            }
            finally
            {
                con.Close();
            }

            getCategories(); // refresh table after insert/update

        }

        private void ShowMessage(string message, bool isSuccess)
        {
            lblMsg.Visible = true;
            lblMsg.Text = message;
            lblMsg.CssClass = isSuccess ? "alert alert-success" : "alert alert-danger";
        }
        protected void btnClear_Click(object sender, EventArgs e)
        {
            clear();
            lblMsg.Visible = false;
        }

        void clear()
        {
            txtCategoryName.Text = string.Empty;
            cbIsActive.Checked = false;
            hfCategoryId.Value = "0";
            imagePreview.ImageUrl = string.Empty;
        }
    }
}