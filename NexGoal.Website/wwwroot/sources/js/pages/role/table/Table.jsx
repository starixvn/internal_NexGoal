import React from 'react';

import getResource from '../../../resourceHelper';
import Row from './Row';

const Table = ({ functions, isHost, onChange }) => {

    return (
        <table className="table table-bordered assign-permission">
            <thead>
                <tr>
                    <th rowSpan="2">{getResource('Function')}</th>
                    <th colSpan="2">{getResource('RolePermission')}</th>
                </tr>
                <tr>
                    <th>{getResource('Read')}</th>
                    <th>{getResource('Host')}</th>
                </tr>
            </thead>
            <tbody>
                {
                    functions.map(func =>
                        <Row key={'function-' + func.functionId}
                            function={func}
                            isHost={isHost}
                            onChange={onChange}
                        />
                    )
                }
            </tbody>
        </table>
    );
};

export default Table;