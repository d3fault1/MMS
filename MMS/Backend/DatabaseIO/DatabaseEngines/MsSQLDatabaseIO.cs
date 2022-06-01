using MMS.DataModels;
using System;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMS.Backend.DatabaseIO
{
    class MsSQLDatabaseIO : IDatabaseIO
    {
        string connectionString = "";
        string masterConnectionString = "";
        public DatabaseCheckResult CheckDatabaseValidity()
        {
            Logging.Debug($"Attempting database validity check.");
            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT TOP 1 * FROM museum_node, museum_nodelog, museum_exhibit, museum_zone, museum_floor, museum_nodestatus";
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    reader.Close();
                }
                Logging.Debug("Database validity check passed successfully");
                return DatabaseCheckResult.OK;
            }
            catch (Exception e)
            {
                if (e is SqlException se)
                {

                    if (se.Number == 4060)
                    {
                        Logging.Debug("Database not found");
                        return DatabaseCheckResult.NotFound;
                    }
                    Logging.Debug("Database found. Checking Validity");
                    if (se.Number == 208)
                    {
                        Logging.Debug("Database not configured properly");
                        return DatabaseCheckResult.TableCorrupt;
                    }
                    Logging.Debug($"Database check error. Exception No. {se.Number} Exception: {e.Message}");
                }

                Logging.Debug($"Database check error. Exception: {e.Message}");
                return DatabaseCheckResult.Exception;
            }
        }

        public bool CreateDatabase()
        {
            try
            {
                string dbname = "museum_database";
                Logging.Debug($"Attempting SQL Create Database {dbname}");
                string sql = $"CREATE DATABASE {dbname}";
                using (var conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sql, conn);
                    command.ExecuteNonQuery();
                }
                Logging.Debug("SQL Create Database successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL Create Database failed. {e.Message}");
                return false;
            }
        }

        public bool CreateTables()
        {
            string table = "";
            string sql = "";
            try
            {
                Logging.Debug($"Attempting Database Tables Creation");
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand command = new SqlCommand(sql, conn);

                    table = "museum_floor";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"name\" VARCHAR(100) NOT NULL," +
                                 "\"description\" VARCHAR(600) NOT NULL," +
                                 "\"is_active\" BOOLEAN NOT NULL," +
                                 "\"image\" VARCHAR(100))";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    table = "museum_zone";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"floor_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_floor(id)," +
                                 "\"name\" VARCHAR(100) NOT NULL," +
                                 "\"description\" VARCHAR(600) NOT NULL," +
                                 "\"is_active\" BOOLEAN NOT NULL," +
                                 "\"image\" VARCHAR(100))";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    table = "museum_exhibit";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"zone_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_zone(id)," +
                                 "\"name\" VARCHAR(100) NOT NULL," +
                                 "\"description\" VARCHAR(600) NOT NULL," +
                                 "\"is_active\" BOOLEAN NOT NULL," +
                                 "\"is_exhibit_show\" BOOLEAN NOT NULL," +
                                 "\"image\" VARCHAR(100))";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    table = "museum_node";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"name\" VARCHAR(100)," +
                                 "\"node_name\" VARCHAR(100)," +
                                 "\"description\" VARCHAR(300)," +
                                 "\"ip\" VARCHAR(100) NOT NULL," +
                                 "\"is_active\" BOOLEAN NOT NULL," +
                                 "\"is_config\" BOOLEAN NOT NULL," +
                                 "\"os_type\" VARCHAR(150)," +
                                 "\"mac_addr\" VARCHAR(100) UNIQUE NOT NULL," +
                                 "\"port\" INT NOT NULL," +
                                 "\"unique_reg_code\" VARCHAR(300) UNIQUE NOT NULL," +
                                 "\"os_name\" VARCHAR(255)," +
                                 "\"os_arch\" VARCHAR(255)," +
                                 "\"total_disc_space\" FLOAT," +
                                 "\"total_cpu\" FLOAT," +
                                 "\"total_ram\" FLOAT," +
                                 "\"exhibit_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_exhibit(id)," +
                                 "\"zone_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_zone(id)," +
                                 "\"floor_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_floor(id)," +
                                 "\"content_metadata\" VARCHAR(MAX)," +
                                 "\"pem_file\" VARCHAR(MAX)," +
                                 "\"heartbeat_rate\" INT NOT NULL," +
                                 "\"image\" VARCHAR(100)," +
                                 "\"is_online\" BOOLEAN NOT NULL," +
                                 "\"total_videos\" INT," +
                                 "\"video_list\" VARCHAR(MAX)," +
                                 "\"sequence_id\" INT NOT NULL," +
                                 "\"is_audio_guide\" BOOLEAN NOT NULL," +
                                 "\"category\" VARCHAR(100)," +
                                 "\"is_control_panel\" BOOLEAN NOT NULL)";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    table = "museum_nodestatus";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"node_id\" BIGINT UNIQUE NOT NULL FOREIGN KEY REFERENCES museum_node(id)," +
                                 "\"version\" VARCHAR(25)," +
                                 "\"temperature\" FLOAT," +
                                 "\"cpu_usage\" INT," +
                                 "\"disc_space_usage\" INT," +
                                 "\"ram_usage\" INT," +
                                 "\"current_timestamp\" FLOAT," +
                                 "\"current_video_name\" VARCHAR(500)," +
                                 "\"current_video_number\" INT," +
                                 "\"current_video_status\" VARCHAR(100)," +
                                 "\"current_volume\" INT," +
                                 "\"video_duration\" FLOAT," +
                                 "\"uptime\" FLOAT)";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                    table = "museum_nodelog";
                    sql = $"CREATE TABLE \"{table}\" (" +
                                 "\"id\" BIGINT NOT NULL IDENTITY PRIMARY KEY," +
                                 "\"node_id\" BIGINT NOT NULL FOREIGN KEY REFERENCES museum_node(id)," +
                                 "\"temperature\" FLOAT," +
                                 "\"uptime\" FLOAT," +
                                 "\"cpu_usage\" INT," +
                                 "\"disc_space_usage\" INT," +
                                 "\"ram_usage\" INT," +
                                 "\"version\" VARCHAR(25))";
                    Logging.Debug($"Attempting SQL Create Table {table}");
                    command.CommandText = sql;
                    command.ExecuteNonQuery();

                }
                Logging.Debug("SQL Create Table successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL Create Table failed on table {table}. {e.Message}");
                return false;
            }
        }

        public List<ExhibitModel> ReadExhibitData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_exhibit");
                List<ExhibitModel> retval = new List<ExhibitModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_exhibit";
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int ord;
                            var row = new ExhibitModel();
                            ord = reader.GetOrdinal("id");
                            row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("zone_id");
                            row.ZoneID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("name");
                            row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("description");
                            row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("is_active");
                            row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("is_exhibit_show");
                            row.IsExhibitShow = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("image");
                            row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            retval.Add(row);
                        }
                    }
                    reader.Close();
                }
                Logging.Debug("SQL SELECT FROM museum_exhibit successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_exhibit failed. {e.Message}");
                return null;
            }
        }

        public List<ZoneModel> ReadZoneData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_zone");
                List<ZoneModel> retval = new List<ZoneModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_zone";
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int ord;
                            var row = new ZoneModel();
                            ord = reader.GetOrdinal("id");
                            row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("floor_id");
                            row.FloorID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("name");
                            row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("description");
                            row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("is_active");
                            row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("image");
                            row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            retval.Add(row);
                        }
                    }
                    reader.Close();
                }
                Logging.Debug("SQL SELECT FROM museum_zone successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_zone failed. {e.Message}");
                return null;
            }
        }

        public List<FloorModel> ReadFloorData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_floor");
                List<FloorModel> retval = new List<FloorModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_floor";
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int ord;
                            var row = new FloorModel();
                            ord = reader.GetOrdinal("id");
                            row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("name");
                            row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("description");
                            row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("is_active");
                            row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("image");
                            row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            retval.Add(row);
                        }
                    }
                    reader.Close();
                }
                Logging.Debug("SQL SELECT FROM museum_floor successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_floor failed. {e.Message}");
                return null;
            }
        }

        public List<NodeModel> ReadNodeData()
        {
            try
            {
                Logging.Debug($"SQL Attempting SELECT FROM museum_node");
                List<NodeModel> retval = new List<NodeModel>();
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = $"SELECT * FROM museum_node";
                    SqlCommand command = new SqlCommand(sql, conn);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int ord;
                            var row = new NodeModel();
                            ord = reader.GetOrdinal("id");
                            row.ID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("name");
                            row.Name = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("node_name");
                            row.NodeName = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("description");
                            row.Description = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("ip");
                            row.IP = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("is_active");
                            row.IsActive = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("is_config");
                            row.IsConfig = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("os_type");
                            row.OSType = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("mac_addr");
                            row.MacAddress = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("port");
                            row.Port = reader.IsDBNull(ord) ? -1 : reader.GetInt32(ord);
                            ord = reader.GetOrdinal("unique_reg_code");
                            row.RegKey = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("os_name");
                            row.OSName = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("os_arch");
                            row.OSArchitecture = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("total_disk_space");
                            row.TotalDiskSpace = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                            ord = reader.GetOrdinal("total_cpu");
                            row.TotalCPU = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                            ord = reader.GetOrdinal("total_ram");
                            row.TotalRam = reader.IsDBNull(ord) ? 0 : reader.GetDouble(ord);
                            ord = reader.GetOrdinal("exhibit_id");
                            row.ExhibitID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("zone_id");
                            row.ZoneID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("floor_id");
                            row.FloorID = reader.IsDBNull(ord) ? -1 : reader.GetInt64(ord);
                            ord = reader.GetOrdinal("content_metadata");
                            row.ContentMetadata = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("pem_file");
                            row.PEMFile = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("image");
                            row.Image = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("heartbeat_rate");
                            row.HeartbeatRate = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                            ord = reader.GetOrdinal("is_online");
                            row.IsOnline = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("total_videos");
                            row.TotalVideos = reader.IsDBNull(ord) ? 0 : reader.GetInt32(ord);
                            ord = reader.GetOrdinal("sequence_id");
                            row.SequenceID = reader.IsDBNull(ord) ? -1 : reader.GetInt32(ord);
                            ord = reader.GetOrdinal("video_list");
                            row.VideoList = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            ord = reader.GetOrdinal("is_audio_guide");
                            row.IsAudioGuide = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("is_control_panel");
                            row.IsControlPanel = reader.IsDBNull(ord) ? false : reader.GetBoolean(ord);
                            ord = reader.GetOrdinal("category");
                            row.Category = reader.IsDBNull(ord) ? "" : reader.GetString(ord);
                            retval.Add(row);
                        }
                    }
                    reader.Close();
                }
                Logging.Debug("SQL SELECT FROM museum_node successful");
                return retval;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL SELECT FROM museum_node failed. {e.Message}");
                return null;
            }
        }

        public bool WriteNodeLogData(List<NodeLogModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_nodelog");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_nodelog(" +
                        $"node_id, temperature, uptime, cpu_usage, disk_space_usage," +
                        $"ram_usage, version) VALUES(@node_id, @temperature, @uptime," +
                        $"@cpu_usage, @disk_space_usage, @ram_usage, @version)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach(var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@node_id", row.NodeID));
                        command.Parameters.Add(new SqlParameter("@temperature", row.Temperature));
                        command.Parameters.Add(new SqlParameter("@uptime", row.Uptime.TotalSeconds));
                        command.Parameters.Add(new SqlParameter("@cpu_usage", row.ProcessorUsage));
                        command.Parameters.Add(new SqlParameter("@disk_space_usage", row.DiskSpaceUsage));
                        command.Parameters.Add(new SqlParameter("@ram_usage", row.RamUsage));
                        command.Parameters.Add(new SqlParameter("@version", row.Version));
                        command.ExecuteNonQuery();
                    }
                    
                }
                Logging.Debug("SQL INSERT INTO museum_nodelog successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_nodelog failed. {e.Message}");
                return false;
            }
        }

        public bool WriteNodeStatusData(List<NodeModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_nodestatus");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_nodestatus(" +
                        $"node_id, temperature, uptime, cpu_usage, disk_space_usage," +
                        $"ram_usage, version, current_timestamp, current_video_name," +
                        $"current_video_number, current_video_status, current_volume, video_duration)" +
                        $"VALUES(@node_id, @temperature, @uptime, @cpu_usage, @disk_space_usage," +
                        $"@ram_usage, @version, @current_timestamp, @current_video_name," +
                        $"@current_video_number, @current_video_status, @current_volume, @video_duration)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach (var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@node_id", row.CurrentStatus.NodeID));
                        command.Parameters.Add(new SqlParameter("@temperature", row.CurrentStatus.Temperature));
                        command.Parameters.Add(new SqlParameter("@uptime", row.CurrentStatus.Uptime.TotalSeconds));
                        command.Parameters.Add(new SqlParameter("@cpu_usage", row.CurrentStatus.ProcessorUsage));
                        command.Parameters.Add(new SqlParameter("@disk_space_usage", row.CurrentStatus.DiskSpaceUsage));
                        command.Parameters.Add(new SqlParameter("@ram_usage", row.CurrentStatus.RamUsage));
                        command.Parameters.Add(new SqlParameter("@version", row.CurrentStatus.Version));
                        command.Parameters.Add(new SqlParameter("@current_timestamp", row.CurrentStatus.TimeStamp.TotalSeconds));
                        command.Parameters.Add(new SqlParameter("@current_video_name", row.CurrentStatus.VideoName));
                        command.Parameters.Add(new SqlParameter("@current_video_number", row.CurrentStatus.VideoNumber));
                        command.Parameters.Add(new SqlParameter("@current_video_status", row.CurrentStatus.VideoStatus));
                        command.Parameters.Add(new SqlParameter("@current_volume", row.CurrentStatus.Volume));
                        command.Parameters.Add(new SqlParameter("@video_duration", row.CurrentStatus.VideoDuration));
                        command.ExecuteNonQuery();
                    }

                }
                Logging.Debug("SQL INSERT INTO museum_nodestatus successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_nodestatus failed. {e.Message}");
                return false;
            }
        }

        public bool WriteNodeData(List<NodeModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_node");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_node(" +
                        $"name, node_name, description, ip, is_active," +
                        $"is_config, os_type, mac_addr, port, unique_reg_code," +
                        $"os_name, os_arch, total_disc_space, total_cpu," +
                        $"total_ram, exhibit_id, zone_id, floor_id, content_metadata," +
                        $"pem_file, heartbeat_rate, image, is_online, total_videos," +
                        $"video_list, sequence_id, is_audio_guide, category, is_control_panel)" +
                        $"VALUES(@name, @node_name, @description, @ip, @is_active, @is_config," +
                        $"@os_type, @mac_addr, @port, @unique_reg_code, @os_name, @os_arch," +
                        $"@total_disc_space, @total_cpu, @total_ram, @exhibit_id, @zone_id," +
                        $"@floor_id, @content_metadata, @pem_file, @heartbeat_rate, @image," +
                        $"@is_online, @total_videos, @video_list, @sequence_id, @is_audio_guide," +
                        $"@category, @is_control_panel)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach (var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@name", row.Name));
                        command.Parameters.Add(new SqlParameter("@node_name", row.NodeName));
                        command.Parameters.Add(new SqlParameter("@description", row.Description));
                        command.Parameters.Add(new SqlParameter("@ip", row.IP));
                        command.Parameters.Add(new SqlParameter("@is_active", row.IsActive));
                        command.Parameters.Add(new SqlParameter("@is_config", row.IsConfig));
                        command.Parameters.Add(new SqlParameter("@os_type", row.OSType));
                        command.Parameters.Add(new SqlParameter("@mac_addr", row.MacAddress));
                        command.Parameters.Add(new SqlParameter("@port", row.Port));
                        command.Parameters.Add(new SqlParameter("@unique_reg_code", row.RegKey));
                        command.Parameters.Add(new SqlParameter("@os_name", row.OSName));
                        command.Parameters.Add(new SqlParameter("@os_arch", row.OSArchitecture));
                        command.Parameters.Add(new SqlParameter("@total_disc_space", row.TotalDiskSpace));
                        command.Parameters.Add(new SqlParameter("@total_cpu", row.TotalCPU));
                        command.Parameters.Add(new SqlParameter("@total_ram", row.TotalRam));
                        command.Parameters.Add(new SqlParameter("@exhibit_id", row.ExhibitID));
                        command.Parameters.Add(new SqlParameter("@zone_id", row.ZoneID));
                        command.Parameters.Add(new SqlParameter("@floor_id", row.FloorID));
                        command.Parameters.Add(new SqlParameter("@content_metadata", row.ContentMetadata));
                        command.Parameters.Add(new SqlParameter("@pem_file", row.PEMFile));
                        command.Parameters.Add(new SqlParameter("@heartbeat_rate", row.HeartbeatRate));
                        command.Parameters.Add(new SqlParameter("@image", row.Image));
                        command.Parameters.Add(new SqlParameter("@is_online", row.IsOnline));
                        command.Parameters.Add(new SqlParameter("@total_videos", row.TotalVideos));
                        command.Parameters.Add(new SqlParameter("@video_list", row.VideoList));
                        command.Parameters.Add(new SqlParameter("@sequence_id", row.SequenceID));
                        command.Parameters.Add(new SqlParameter("@is_audio_guide", row.IsAudioGuide));
                        command.Parameters.Add(new SqlParameter("@category", row.Category));
                        command.Parameters.Add(new SqlParameter("@is_control_panel", row.IsControlPanel));
                        command.ExecuteNonQuery();
                    }
                }
                Logging.Debug("SQL INSERT INTO museum_node successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_node failed. {e.Message}");
                return false;
            }
        }

        public bool WriteFloorData(List<FloorModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_floor");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_floor(" +
                        $"name, description, is_active, image)" +
                        $"VALUES(@name, @description, @is_active, @image)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach (var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@name", row.Name));
                        command.Parameters.Add(new SqlParameter("@description", row.Description));
                        command.Parameters.Add(new SqlParameter("@is_active", row.IsActive));
                        command.Parameters.Add(new SqlParameter("@image", row.Image));
                        command.ExecuteNonQuery();
                    }

                }
                Logging.Debug("SQL INSERT INTO museum_floor successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_floor failed. {e.Message}");
                return false;
            }
        }

        public bool WriteZoneData(List<ZoneModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_zone");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_zone(" +
                        $"name, description, is_active, image, floor_id)" +
                        $"VALUES(@name, @description, @is_active, @image, @floor_id)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach (var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@name", row.Name));
                        command.Parameters.Add(new SqlParameter("@description", row.Description));
                        command.Parameters.Add(new SqlParameter("@is_active", row.IsActive));
                        command.Parameters.Add(new SqlParameter("@image", row.Image));
                        command.Parameters.Add(new SqlParameter("@floor_id", row.FloorID));
                        command.ExecuteNonQuery();
                    }

                }
                Logging.Debug("SQL INSERT INTO museum_zone successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_zone failed. {e.Message}");
                return false;
            }
        }

        public bool WriteExhibitData(List<ExhibitModel> data)
        {
            try
            {
                Logging.Debug($"SQL Attempting INSERT INTO museum_exhibit");
                using (var conn = new SqlConnection(connectionString))
                {
                    string sql = $"INSERT INTO museum_exhibit(" +
                        $"name, description, is_active, is_exhibit_show, image, zone_id)" +
                        $"VALUES(@name, @description, @is_active, @is_exhibit_show, @image, @zone_id)";
                    SqlCommand command = new SqlCommand(sql, conn);
                    foreach (var row in data)
                    {
                        command.Parameters.Clear();
                        command.Parameters.Add(new SqlParameter("@name", row.Name));
                        command.Parameters.Add(new SqlParameter("@description", row.Description));
                        command.Parameters.Add(new SqlParameter("@is_active", row.IsActive));
                        command.Parameters.Add(new SqlParameter("@is_exhibit_show", row.IsExhibitShow));
                        command.Parameters.Add(new SqlParameter("@image", row.Image));
                        command.Parameters.Add(new SqlParameter("@zone_id", row.ZoneID));
                        command.ExecuteNonQuery();
                    }

                }
                Logging.Debug("SQL INSERT INTO museum_exhibit successful");
                return true;
            }
            catch (Exception e)
            {
                Logging.Debug($"SQL INSERT INTO museum_exhibit failed. {e.Message}");
                return false;
            }
        }
    }
}
