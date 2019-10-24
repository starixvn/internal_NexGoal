import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import { getNominationClassName, getRoleClassName, resolveClientUrl, calendar } from '../../common-functions';
import { NominationStatuses, PerformanceRecordType } from '../../constants';
import getResource from '../../resourceHelper';

const PageType = {
    MyPage: 1,
    MemberPage: 2,
    MemberProfile: 3
};

export default class PerformanceRecord extends Component {
    static propTypes = {
        data: PropTypes.object,
        isHighlighted: PropTypes.bool,
        onDelete: PropTypes.func,
        onEdit: PropTypes.func,
        onEditDraft: PropTypes.func,
        pageType: PropTypes.number
    };

    static defaultProps = {
        data: null,
        pageType: PageType.MyPage,
        isHighlighted: false,
        onEdit: function () {
            //empty
        },
        onEditDraft: function () {
            //empty
        },
        onDelete: function () {
            //empty
        }
    };

    constructor(props) {
        super(props);

        this.state = {
            showCollapseExpandButton: false,
            isExpanded: false
        };
    }

    componentDidMount() {
        const record = this.props.data;
        if (record && (record.typeId === PerformanceRecordType.PerformanceCheckin || record.typeId === PerformanceRecordType.Nomination)) {
            this.detectCollapseExpandComment();
        }
    }

    shouldComponentUpdate(nextProps, nextState) {
        if (JSON.stringify(this.props.data) !== JSON.stringify(nextProps.data)
            || this.props.pageType !== nextProps.pageType
            || this.props.isHighlighted !== nextProps.isHighlighted
            || this.state.showCollapseExpandButton !== nextState.showCollapseExpandButton
            || this.state.isExpanded !== nextState.isExpanded) {
            return true;
        }

        return false;
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    }

    detectCollapseExpandComment() {
        const height = this.element.offsetHeight;
        if (height > 60) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    }

    handleEdit = () => this.props.onEdit(this.props.data.performanceRecordId);

    handleEditDraft = () => this.props.onEditDraft(this.props.data.performanceRecordId);

    handleDelete = () => this.props.onDelete(this.props.data.performanceRecordId);

    renderPerformanceCheckin = () => {
        const record = this.props.data;
        let { employees, creator } = record;
        const grade = record.grade ? `(${record.grade.name}) ${record.grade.description}` : getResource('N/A');
        const projects = record.projects && record.projects.length > 0 ?
            record.projects.map(project => <span key={project.projectId} className="tag">{project.name}</span>) : getResource('N/A');
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';
        const modifiedDate = record.modifiedDate ? calendar(record.modifiedDate) : '';

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';

        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className={`avatar-user ${record.createdDate ? '' : 'small'}`}>
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-checkin" />
                                <img src={employees[0].avatar} alt="avatar" className="avatar" />
                                <span className="role-member">{employees.map(employee => employee.nickName).join(', ')}</span>
                            </Fragment>
                        }
                    </div>
                    {
                        record.isHost &&
                        <div className="date">
                            <div className="edit-date">
                                <a className="icon-edit" title={getResource('Edit')} onClick={this.handleEdit} />
                            </div>
                        </div>
                    }
                </div>
                <div className="date-time">
                    <span className="submit-date">{createdDate}</span>
                    {modifiedDate &&
                        <div className="edited-date">
                            <span className="icon-edited-date" />
                            <span className="edited-tooltip">
                                <i className="arrow" />
                                Edited on {modifiedDate}
                            </span>
                        </div>
                    }
                </div>
                <div className="comment" >
                    <p className={commentWrapperClass} ref={element => this.element = element}>{record.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{grade}</div>
                </div>
                {
                    !this.props.isHideProjectTag &&
                    <div className="project">
                        <div className="lb-project">{`${getResource('ProjectTag')}:`}</div> <div className="project-detail">{projects}</div>
                    </div>
                }
            </div>
        );
    }

    renderDraftPerformanceCheckin = () => {
        const record = this.props.data;
        let { employees, creator } = record;
        const grade = record.grade ? `(${record.grade.name}) ${record.grade.description}` : getResource('N/A');
        const projects = record.projects && record.projects.length > 0 ?
            record.projects.map(project => <span key={project.projectId} className="tag">{project.name}</span>) : getResource('N/A');
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';

        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-draft-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-checkin" />
                                {employees.length === 1 && <img src={employees[0].avatar} alt="avatar" className="avatar" />}
                                {employees.length > 1 && <i className="avatar icon-draft-member" />}
                                <span className="role-member">{employees.map(employee => employee.nickName).join(', ')}</span>
                            </Fragment>
                        }
                    </div>
                    {
                        <div className="date">
                            <div className="edit-date">
                                {
                                    <a className="icon-edit" title={getResource('Edit')} onClick={this.handleEditDraft} />
                                }
                                {
                                    <a className="icon-delete" title={getResource('Delete')} onClick={this.handleDelete} />
                                }
                            </div>
                        </div>
                    }
                </div>
                <div className="date-time">
                    {
                        <span className="draft">draft</span>
                    }
                    <span className="submit-date">{createdDate}</span>

                </div>
                <div className="comment" >
                    <p className={commentWrapperClass} ref={element => this.element = element}>{record.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{grade}</div>
                </div>
                {
                    !this.props.isHideProjectTag &&
                    <div className="project">
                        <div className="lb-project">{`${getResource('ProjectTag')}:`}</div> <div className="project-detail">{projects}</div>
                    </div>
                }
            </div>
        );
    }

    renderNomination = () => {
        const record = this.props.data;
        let { employees, creator } = record;
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';
        const modifiedDate = record.modifiedDate ? calendar(record.modifiedDate) : '';
        const status = NominationStatuses.find(x => x.value === record.nominationStatusId);
        const endDateForNominator = moment(record.nominationPeriod.endDateForNominator);
        const currentDate = moment();

        let commentWrapperClass = 'expanded';
        let expandCommentButtonClass = 'see-less';

        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
            expandCommentButtonClass = 'see-more';
        }

        return (
            <div className={`record ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {
                            employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
                            <Fragment>
                                <span className="icon-nomination" />
                                <img src={employees[0].avatar} alt="avatar" className="avatar" />
                                <span className="role-member">{employees.map(employee => employee.nickName).join(', ')}</span>
                            </Fragment>
                        }

                    </div>
                    <div className="date">
                        <div className="edit-date">
                            <div className="nomination-info">
                                <div className="nomination-period">{record.nominationPeriod && record.nominationPeriod.title}</div>

                            </div>
                            {
                                record.isHost &&
                                <Fragment>
                                    <a className="icon-edit" title={getResource('Edit')} onClick={this.handleEdit} />
                                    <a className="icon-delete" title={getResource('Delete')} onClick={this.handleDelete} />
                                </Fragment>
                            }
                            {
                                endDateForNominator < currentDate && status &&
                                <div className={getNominationClassName(record.nominationStatusId)}>{status.text}</div>
                            }
                        </div>
                    </div>
                </div>
                <div className="date-time">
                    <span className="submit-date">{createdDate}</span>
                    {
                        modifiedDate && endDateForNominator >= currentDate &&
                        <div className="edited-date">
                            <span className="icon-edited-date" />
                            <span className="edited-tooltip"><i className="arrow" />Edited on {modifiedDate}</span>
                        </div>
                    }
                </div>

                <div className="comment" >
                    <p className={commentWrapperClass} ref={element => this.element = element}>{record.comment}</p>
                    {
                        this.state.showCollapseExpandButton ?
                            <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />
                            : null
                    }
                </div>
            </div >
        );
    }

    renderSystemPerformanceAppraisal = () => {
        const record = this.props.data;
        const grade = record.grade ? `(${record.grade.name}) ${record.grade.description}` : getResource('N/A');
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';

        return (
            <div className={`record record-system ${this.props.isHighlighted ? 'highlighted' : ''}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <div className="avatar nexgoal-system"><img src={resolveClientUrl('dist/avatars/icon-nexgoal.svg')} alt="avatar" /></div>
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>NexGoal - {record.period ? record.period.name : ''}</span>
                        <div className="submit-date">{createdDate}</div>
                    </div>
                </div>

                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{grade}</div>
                </div>
            </div>
        );
    }

    getNominationContent = nominationStatusId => {
        const record = this.props.data;
        const nominationPeriod = record.nominationPeriod ? record.nominationPeriod.title : '';
        const employees = record.employees.map(employee => employee.nickName).join(', ');

        switch (nominationStatusId) {
            case NominationStatuses[0].value:
                return this.props.pageType === PageType.MyPage ?
                    `Congratulations! You were nominated for the ${nominationPeriod} award.`
                    : this.props.pageType === PageType.MemberProfile ?
                        `${employees} was nominated for the ${nominationPeriod} award.`
                        : <Fragment>Here is the list of your members who got into the Nominated round: <span className="bold">{employees}</span></Fragment>;

            case NominationStatuses[1].value:
                return this.props.pageType === PageType.MyPage ?
                    `Congratulations! You were shortlisted for the ${nominationPeriod} award.`
                    : this.props.pageType === PageType.MemberProfile ?
                        `${employees} was shortlisted for the ${nominationPeriod} award.`
                        : <Fragment>Here is the list of your members who got into the Shortlisted round: <span className="bold">{employees}</span></Fragment>;

            case NominationStatuses[2].value:
                return this.props.pageType === PageType.MyPage ?
                    `Congratulations! You were the Winner of ${nominationPeriod} award.`
                    : this.props.pageType === PageType.MemberProfile ?
                        `Congratulations! ${employees} was the Winner of ${nominationPeriod} award.`
                        : <Fragment>Here is the list of your members who are the Winner of this Award: <span className="bold">{employees}</span></Fragment>;
        }
    }

    renderSystemNomination = () => {
        const record = this.props.data;
        const nominationStatusId = record.nominationStatusId;
        const nominationStatus = NominationStatuses.find(x => x.value === nominationStatusId);
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';
        const content = this.getNominationContent(nominationStatusId);

        return (
            <div className={`record record-system ${getNominationClassName(nominationStatusId)} ${this.props.isHighlighted ? 'highlighted' : ''}`}
                id={`record-${record.performanceRecordId}`}
            >
                {
                    this.props.pageType === PageType.MyPage &&
                    <div className="comment">{content}</div>
                }
                {
                    this.props.pageType === PageType.MemberProfile &&
                    <div className="comment">{content}</div>
                }
                {
                    this.props.pageType === PageType.MemberPage &&
                    <Fragment>
                        <div className="avatar-user">
                            <span className={`icon-${getNominationClassName(nominationStatusId)} ${record.nominationPeriod.quarter ? 'eoq' : 'eoy'} avatar`} />
                            <div className="person-info">
                                <span className={getRoleClassName(record.roleId)}>
                                    {record.nominationPeriod && record.nominationPeriod.title}
                                </span>
                                <div>
                                    <span className="submit-date">{createdDate}</span>
                                </div>
                            </div>
                            <div className="date">
                                <div className={getNominationClassName(nominationStatusId)}>
                                    {nominationStatus ? nominationStatus.text : ''}
                                </div>
                            </div>
                        </div>
                        <div className="comment">{content}</div>
                    </Fragment>
                }
            </div>
        );
    }

    renderRecord = () => {
        const record = this.props.data;

        switch (record.typeId) {
            case PerformanceRecordType.PerformanceCheckin:
            case PerformanceRecordType.PerformanceAppraisal:
                if (record.isDraft) {
                    return this.renderDraftPerformanceCheckin();
                }
                else {
                    return this.renderPerformanceCheckin();
                }

            case PerformanceRecordType.Nomination:
                return this.renderNomination();

            case PerformanceRecordType.SystemPerformanceAppraisal:
                return this.renderSystemPerformanceAppraisal();

            case PerformanceRecordType.SystemNomination:
                return this.renderSystemNomination();
        }

        return (<Fragment />);
    }

    render() {
        return (
            <Fragment>
                {
                    this.props.data && this.renderRecord()
                }
            </Fragment>
        );
    }
}