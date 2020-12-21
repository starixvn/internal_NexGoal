import React, { Component, Fragment } from 'react';
import PropTypes from 'prop-types';
import moment from 'moment';
import isEqual from 'react-fast-compare';

import { getNominationClassName, getRoleClassName, resolveClientUrl, calendar, getPAGradeClassName, groupBy } from '../../common-functions';
import { NominationStatuses, PerformanceRecordType, PerformanceProcess, EmptyPlaceHolder } from '../../constants';
import getResource from '../../resourceHelper';
import GradeSummary from '../../shared/components/performance-appraisal/GradeSummary';
import Interaction from '../../shared/components/pr-interaction/Interaction';

const PageType = {
    MyPage: 1,
    MemberPage: 2,
    MemberProfile: 3
};

export default class PerformanceRecord extends Component {
    static propTypes = {
        data: PropTypes.objectOf(PropTypes.object),
        isHighlighted: PropTypes.bool,
        isHideInteraction: PropTypes.bool,
        onDelete: PropTypes.func,
        onEdit: PropTypes.func,
        onEditDraft: PropTypes.func,
        pageType: PropTypes.number
    };

    static defaultProps = {
        data: null,
        pageType: PageType.MyPage,
        isHighlighted: false,
        isHideInteraction: false,
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
        return !isEqual(this.state, nextState) || !isEqual(this.props, nextProps);
    }

    collapseExpandComment = () => {
        this.setState({
            isExpanded: !this.state.isExpanded
        });
    };

    detectCollapseExpandComment() {
        const height = this.element.offsetHeight;
        if (height > 60) {
            this.setState({
                showCollapseExpandButton: true,
                isExpanded: true
            });
        }
    };

    handleEdit = () => this.props.onEdit(this.props.data.performanceRecordId);

    handleEditDraft = () => this.props.onEditDraft(this.props.data.performanceRecordId);

    handleDelete = () => this.props.onDelete(this.props.data.performanceRecordId);

    renderMembers = (employee, i) => {
        return (
            <Fragment key={employee.employeeId}>
                <a href={resolveClientUrl(`member/details/${employee.employeeId}`)}>
                    {employee.nickName}
                </a>
                {i < this.props.data.employees.length - 1 && ', '}
            </Fragment>
        );
    };

    isHideInteraction = (record, creator) => {
        const currentUserCode = document.getElementById('employeeInfo-code').value;
        return this.props.isHideInteraction || (creator.code === currentUserCode && record.typeId === 1 && record.roleId === 3);
    };

    getHighlightedCssClass = () => this.props.isHighlighted ? 'highlighted' : '';

    renderCreatorForPerformanceCheckin = (employees, creator) => {
        return this.props.pageType === PageType.MemberPage && employees && employees.some(x => x.employeeId === creator.employeeId) ?
            <a href={resolveClientUrl(`member/details/${creator.employeeId}`)} >{creator.nickName}</a>
            : creator.nickName;
    };

    renderMemberForPerformanceCheckin = (employees, creator) => {
        return employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
            <Fragment>
                <span className="icon-checkin" />
                <img src={employees[0].avatar} alt="avatar" className="avatar" />
                <span className="role-member">
                    {this.props.pageType === PageType.MemberPage ? employees.map(this.renderMembers) : employees.map(e => e.nickName).join(', ')}
                </span>
            </Fragment>;
    };

    renderMemberForDraftPerformanceCheckin = (employees, creator) => {
        return employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
            <Fragment>
                <span className="icon-checkin" />
                {employees.length === 1 && <img src={employees[0].avatar} alt="avatar" className="avatar" />}
                {employees.length > 1 && <i className="avatar icon-draft-member" />}
                <span className="role-member">
                    {this.props.pageType === PageType.MemberPage ? employees.map(this.renderMembers) : employees.map(e => e.nickName).join(', ')}
                </span>
            </Fragment>;
    };

    renderMemberForNomination = (employees, creator) => {
        return employees && employees.length > 0 && employees[0].employeeId !== creator.employeeId &&
            <Fragment>
                <span className="icon-nomination" />
                <img src={employees[0].avatar} alt="avatar" className="avatar" />
                <span className="role-member">{employees.map(employee => employee.nickName).join(', ')}</span>
            </Fragment>;
    };

    renderPerformanceGrade = grade => grade ? `(${grade.name}) ${grade.description}` : getResource('N/A');

    renderExpandCommentButton = () => {
        if (this.state.showCollapseExpandButton) {
            const expandCommentButtonClass = this.state.isExpanded ? 'see-more' : 'see-less';
            return <span className={expandCommentButtonClass} onClick={this.collapseExpandComment} />;
        }

        return null;
    };

    renderProject = projects => {
        return projects && projects.length > 0 ?
            projects.map(project => <span key={project.projectId} className="tag">{project.name}</span>) : getResource('N/A');
    };

    renderPerformanceCheckin = () => {
        const record = this.props.data;
        const { employees, creator } = record;
        const isHideInteraction = this.isHideInteraction(record, creator);

        let commentWrapperClass = 'expanded';
        if (this.state.isExpanded) {
            commentWrapperClass = 'collapsed';
        }

        return (
            <div className={`record ${this.getHighlightedCssClass()}`} id={`record-${record.performanceRecordId}`}>
                <div className={`avatar-user ${record.createdDate ? '' : 'small'}`}>
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>
                            {this.renderCreatorForPerformanceCheckin(employees, creator)}
                        </span>
                        {this.renderMemberForPerformanceCheckin(employees, creator)}
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
                    <span className="submit-date">{calendar(record.createdDate)}</span>
                    {
                        record.modifiedDate &&
                        <div className="edited-date">
                            <span className="icon-edited-date" />
                            <span className="edited-tooltip">
                                <i className="arrow" />
                                Edited on {calendar(record.modifiedDate)}
                            </span>
                        </div>
                    }
                </div>
                <div className="comment" >
                    <p className={commentWrapperClass} ref={element => this.element = element}>{record.comment}</p>
                    {
                        this.renderExpandCommentButton()
                    }
                </div>
                <div className="grade">
                    {`${getResource('Grade')}:`} <div className="grade-detail">{this.renderPerformanceGrade(record.grade)}</div>
                </div>
                {
                    record.typeId === PerformanceRecordType.PerformanceAppraisal &&
                    <GradeSummary processes={this.props.processes} toeic={this.props.toeic} toeicPolicy={this.props.toeicPolicy} />
                }
                {
                    !this.props.isHideProjectTag &&
                    <div className="project">
                        <div className="lb-project">{`${getResource('ProjectTag')}:`}</div> <div className="project-detail">
                            {this.renderProject(record.projects)}
                        </div>
                    </div>
                }
                {
                    !isHideInteraction &&
                    <Interaction className="pr-comment"
                        creator={creator}
                        isReadOnly={this.props.feedbackReadOnly || !record.isAllowEditFeedback}
                        isDisableComment={record.typeId !== 1 || record.roleId !== 2}
                        recordId={record.performanceRecordId}
                        reactions={record.reactions}
                        feedbacks={record.feedbacks}
                    />
                }
            </div>
        );
    };

    renderDraftPerformanceCheckin = () => {
        const record = this.props.data;
        const { employees, creator } = record;

        let expandCommentButtonClass = 'see-less';
        let commentWrapperClass = 'expanded';
        if (this.state.isExpanded) {
            expandCommentButtonClass = 'see-more';
            commentWrapperClass = 'collapsed';
        }

        return (
            <div className={`record ${this.getHighlightedCssClass()}`} id={`record-draft-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {this.renderMemberForDraftPerformanceCheckin(employees, creator)}
                    </div>
                    {
                        <div className="date">
                            <div className="edit-date">
                                {<a className="icon-edit" title={getResource('Edit')} onClick={this.handleEditDraft} />}
                                {<a className="icon-delete" title={getResource('Delete')} onClick={this.handleDelete} />}
                            </div>
                        </div>
                    }
                </div>
                <div className="date-time">
                    {<span className="draft">draft</span>}
                    <span className="submit-date">{calendar(record.createdDate)}</span>
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
                    {`${getResource('Grade')}:`} <div className="grade-detail">{this.renderPerformanceGrade(record.grade)}</div>
                </div>
                {
                    !this.props.isHideProjectTag &&
                    <div className="project">
                        <div className="lb-project">{`${getResource('ProjectTag')}:`}</div> <div className="project-detail">
                            {this.renderProject(record.projects)}
                        </div>
                    </div>
                }
            </div>
        );
    };

    renderNomination = () => {
        const record = this.props.data;
        const { employees, creator } = record;
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
            <div className={`record ${this.getHighlightedCssClass()}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <img src={creator.avatar} alt="avatar" className="avatar" />
                    <div className="person-info">
                        <span className={getRoleClassName(record.roleId)}>{creator.nickName}</span>
                        {this.renderMemberForNomination(employees, creator)}
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
                    <span className="submit-date">{calendar(record.createdDate)}</span>
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
    };

    renderPAProcessGrades = gradeDetails => {
        const renderGradeDetails = gradeDetails.filter(x => x.processId !== PerformanceProcess.ProjectManager);
        if (gradeDetails.some(x => x.processId === PerformanceProcess.ProjectManager)) {
            const groupedPMProcess = groupBy(gradeDetails.filter(x => x.processId === PerformanceProcess.ProjectManager && x.grade), x => x.grade);
            renderGradeDetails.splice(1, 0, {
                processId: PerformanceProcess.ProjectManager,
                process: getResource('ProjectManager'),
                grade: groupedPMProcess.length > 0 ? groupedPMProcess.map(g => `${g[1].length}${g[0]}`).join(', ') : getResource('N/A')
            });
        } else {
            renderGradeDetails.splice(1, 0, {
                processId: PerformanceProcess.ProjectManager,
                process: getResource('ProjectManager'),
                grade: EmptyPlaceHolder
            });
        }

        return renderGradeDetails.map(x => {
            let grade = EmptyPlaceHolder;

            if (x.processId === PerformanceProcess.ProjectManager) {
                grade = x.grade;
            } else {
                grade = x.grade ? x.grade : EmptyPlaceHolder;
            }

            return <div className={`process-${x.processId}`} key={x.processId}>
                {x.process}: <div className="grade-detail">{grade}</div>
            </div>;
        });
    };

    renderToeicBonus = toeicPolicy => {
        if (toeicPolicy !== null && toeicPolicy > 0) {
            return '(+1 grade)';
        }

        if (toeicPolicy === null || toeicPolicy < 0) {
            return '(-1 grade)';
        }

        return '';
    };

    showPADetails = paRecordId => {
        this.props.showPaDetail(paRecordId);
    };

    renderSystemPerformanceAppraisal = () => {
        const record = this.props.data;
        const originalGradeId = record.paSummary.originalGrade ? record.paSummary.originalGrade.gradeId : null;

        const gradeChangeClass = getPAGradeClassName(originalGradeId, record.grade ? record.grade.gradeId : null);
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';

        return (
            <div className={`record record-system ${this.getHighlightedCssClass()}`} id={`record-${record.performanceRecordId}`}>
                <div className="avatar-user">
                    <div className="avatar nexgoal-system"><img src={resolveClientUrl('dist/avatars/icon-nexgoal.svg')} alt="avatar" /></div>
                    <div className="person-info">
                        <span onClick={() => this.showPADetails(record.paSummary.paRecordId)}
                                className={`${getRoleClassName(record.roleId)} nextGoalText`}>NexGoal - {record.period ? record.period.name : ''}
                        </span>
                        <div className="submit-date">{createdDate}</div>
                    </div>
                </div>
                {this.renderPAProcessGrades(record.paSummary.gradeDetails)}
                <div className="toeic">
                    TOEIC: {record.paSummary.toeic ?
                        `${record.paSummary.toeic} ${this.renderToeicBonus(record.paSummary.toeicPolicy)}`
                        : EmptyPlaceHolder
                    }
                </div>
                <div className="grade">
                    {`${getResource('FinalGrade')}:`}
                    <div className="grade-detail">
                        {this.renderPerformanceGrade(record.grade)}
                        <span className={gradeChangeClass} />
                    </div>
                </div>
            </div>
        );
    };

    getNominatedContent = (nominationPeriod, employees) => {
        switch (this.props.pageType) {
            case PageType.MyPage:
                return `Congratulations! You were nominated for the ${nominationPeriod} award.`;

            case PageType.MemberProfile:
                return `${employees} was nominated for the ${nominationPeriod} award.`;

            default:
                return <Fragment>
                    Here is the list of your members who got into the Nominated round: <span className="bold">{employees}</span>
                </Fragment>;
        }
    };

    getShortlistedContent = (nominationPeriod, employees) => {
        switch (this.props.pageType) {
            case PageType.MyPage:
                return `Congratulations! You were shortlisted for the ${nominationPeriod} award.`;

            case PageType.MemberProfile:
                return `${employees} was shortlisted for the ${nominationPeriod} award.`;

            default:
                return <Fragment>
                    Here is the list of your members who got into the Shortlisted round: <span className="bold">{employees}</span>
                </Fragment>;
        }
    };

    getWinnerContent = (nominationPeriod, employees) => {
        switch (this.props.pageType) {
            case PageType.MyPage:
                return `Congratulations! You were the Winner of ${nominationPeriod} award.`;

            case PageType.MemberProfile:
                return `Congratulations! ${employees} was the Winner of ${nominationPeriod} award.`;

            default:
                return <Fragment>
                    Here is the list of your members who are the Winner of this Award: <span className="bold">{employees}</span>
                </Fragment>;
        }
    };

    getNominationContent = nominationStatusId => {
        const record = this.props.data;
        const nominationPeriod = record.nominationPeriod ? record.nominationPeriod.title : '';
        const employees = record.employees.map(employee => employee.nickName).join(', ');

        switch (nominationStatusId) {
            case NominationStatuses[0].value:
                return this.getNominatedContent(nominationPeriod, employees);

            case NominationStatuses[1].value:
                return this.getShortlistedContent(nominationPeriod, employees);

            case NominationStatuses[2].value:
                return this.getWinnerContent(nominationPeriod, employees);

            default: return '';
        }
    };

    renderSystemNomination = () => {
        const record = this.props.data;
        const nominationStatusId = record.nominationStatusId;
        const nominationStatus = NominationStatuses.find(x => x.value === nominationStatusId);
        const createdDate = record.createdDate ? calendar(record.createdDate) : '';
        const content = this.getNominationContent(nominationStatusId);

        return (
            <div className={`record record-system ${getNominationClassName(nominationStatusId)} ${this.getHighlightedCssClass()}`}
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
    };

    renderRecord = () => {
        const record = this.props.data;

        switch (record.typeId) {
            case PerformanceRecordType.PerformanceCheckin:
            case PerformanceRecordType.PerformanceAppraisal:
                if (record.isDraft) {
                    return this.renderDraftPerformanceCheckin();
                } else {
                    return this.renderPerformanceCheckin();
                }

            case PerformanceRecordType.Nomination:
                return this.renderNomination();

            case PerformanceRecordType.SystemPerformanceAppraisal:
                return this.renderSystemPerformanceAppraisal();

            case PerformanceRecordType.SystemNomination:
                return this.renderSystemNomination();
        }

        return <Fragment />;
    };

    render() {
        return (
            <Fragment>{this.props.data && this.renderRecord()}</Fragment>
        );
    }
}