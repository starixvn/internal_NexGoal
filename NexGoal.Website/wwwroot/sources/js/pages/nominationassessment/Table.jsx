import React, { Component, Fragment } from 'react';
import Select from 'react-select';

import getResource from '../../resourceHelper';
import { NominationStatuses } from '../../constants';
import { getNominationClassName } from '../../common-functions';

export default class Table extends Component {
    constructor(props) {
        super(props);
    }

    renderNominator = nominator => (
        <tr key={nominator.performanceRecordId}>
            <td>{`${nominator.creator.fullName} (${nominator.creator.nickName})`}</td>
            <td></td>
        </tr>
    )

    renderNomination = (nomination, index) => {
        let rowSpan = nomination.nominators.length + 1;

        let renderStatus = this.props.canUpdate ?
            <Select name="nominationPeriod" className="react-select-container" classNamePrefix="react-select"
                value={NominationStatuses.find(x => x.value === nomination.statusId)} options={NominationStatuses}
                isClearable={false} getOptionLabel={e => e.text}
                onChange={value => this.props.onUpdateStatus(nomination.nominationId, value.value)}
            />
            :
            <span className={getNominationClassName(nomination.statusId)}>
                {NominationStatuses.find(x => x.value === nomination.statusId).text}
            </span>

        return (
            <Fragment key={nomination.nominationId}>
                <tr>
                    <td rowSpan={rowSpan} className="text-center">{index + 1}</td>
                    <td rowSpan={rowSpan}>{`${nomination.nominee.fullName} (${nomination.nominee.nickName})`}</td>
                    <td rowSpan={rowSpan}>{nomination.nominee.groupName}</td>
                    <td rowSpan={rowSpan}>
                        {renderStatus}
                    </td>
                </tr>
                {nomination.nominators.map(this.renderNominator)}
            </Fragment>
        )
    }

    render() {
        return (
            <table className="table table-bordered table-striped">
                <thead>
                    <tr>
                        <th>{getResource('SymbolCount')}</th>
                        <th>{getResource('Nominee')}</th>
                        <th>{getResource('Team')}</th>
                        <th>{getResource('Status')}</th>
                        <th>{getResource('Nominator')}</th>
                        <th>{getResource('Action')}</th>
                    </tr>
                </thead>
                <tbody>
                    {this.props.nominations.map(this.renderNomination)}
                </tbody>
            </table>
        );
    }
}