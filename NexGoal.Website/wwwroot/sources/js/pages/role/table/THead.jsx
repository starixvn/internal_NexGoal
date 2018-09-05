import React from 'react';
import getResource from '../../../resourceHelper';

const THead = () => (
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
);

export default THead;