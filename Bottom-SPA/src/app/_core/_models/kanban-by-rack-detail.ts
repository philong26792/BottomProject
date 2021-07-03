export interface RackDetail {
  rack:string;
  count:number;
  t3:string;
}
export interface KanbanByRackDetail {
  build_ID:string;
  area_ID:string;
  area_Name:string;
  rackDetail:RackDetail[];
}
