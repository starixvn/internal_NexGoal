import React from 'react';

import getResource from '../../../resourceHelper';
import { noDataIndication } from '../../../common-functions';
import Row from './Row';

const Table = ({ tableData, onAddMember, onRemoveMember }) => {
    return (
        <table className="table table-bordered table-striped tbl-pm-assignment">
            <thead>
                <tr>
                    <th>{getResource('Employee')}</th>
                    <th>{getResource('JobTitle')}</th>
                    <th className="text-center col-120">{getResource('Remove')}</th>
                </tr>
            </thead>
            <tbody>
                {
                    tableData.length === 0 &&
                    <tr><td className="text-center" colSpan={3}>{noDataIndication()}</td></tr>
                }
                {
                    tableData.length > 0 &&
                    tableData.map(data =>
                        <Row key={data.pm.employeeId} data={data}
                            onAddMember={onAddMember}
                            onRemoveMember={onRemoveMember}
                        />
                    )
                }
            </tbody>
        </table>
    );
};

export default Table;