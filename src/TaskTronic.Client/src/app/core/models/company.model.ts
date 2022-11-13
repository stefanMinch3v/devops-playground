import { DepartmentModel } from './department.model';

export class CompanyModel {
    constructor(
        public companyId: number,
        public name: string,
        public departments: Array<DepartmentModel>) {}
}
